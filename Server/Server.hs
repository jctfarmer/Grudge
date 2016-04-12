{-# LANGUAGE OverloadedStrings #-}
module Server where

import Game

import Data.Acid
import Data.Maybe
import Data.SafeCopy
import Data.Typeable
import qualified Data.Map.Strict as M
import Control.Concurrent
import Control.Concurrent.MVar
import Control.Concurrent.Chan
import Control.Monad
import Control.Monad.State
import Control.Monad.Reader
import Control.Monad.Fix
import Network.Socket
import GHC.Generics
import Data.Aeson
import Data.Aeson.Types
import qualified Data.ByteString.Lazy.Char8 as C
import System.IO
import System.IO.Error
import System.Random

-- Connections manage game players and their socket handles
type Connection = (ConnPlayer, Handle)

main :: IO ()
main =  do
         sock <- socket AF_INET Stream 0    
         setSocketOption sock ReuseAddr 1   
         bind sock (SockAddrInet 13337 iNADDR_ANY)   
         listen sock 2                              
         comm <- newEmptyMVar
         db <- openLocalState (UserDB M.empty)
         catchIOError (do forkIO $ playQueue [] comm db
                          mainLoop sock comm)
                      (\_ -> return ())
         closeAcidState db

snd' (_, a, _) = a

playQueue :: [VConnection] -> MVar Connection -> AcidState UserDB -> IO ()
playQueue cs mv db = do (c, hdl) <- takeMVar mv
                        let chr = char c
                        new <- query db (Exists c)
                        when new (putStrLn "New player..." >> update db (NewPlayer c))
                        p <- query db (ValidatePlayer c)
                        print p
                        ps <- filterM (hIsOpen . snd') cs
                        print ps
                        case p of
                           Nothing  -> hPutStrLn hdl "{\"error\":\"Invalid Password\"}" 
                                       >> hClose hdl >> playQueue ps mv db
                           (Just p) -> 
                                 do sendStats (p, hdl, chr)
                                    if queued c && isJust chr
                                       then if not . null $ ps 
                                             then do forkIO $ makeMatch (p, hdl, chr) (last ps) db
                                                     playQueue (init ps) mv db
                                             else playQueue ((p,hdl,chr):ps) mv db
                                       else playQueue ps mv db

type VConnection = (Player, Handle, Maybe Character)

sendStats :: VConnection -> IO ()
sendStats (Player n _ w l, hdl, _) = hPutStrLn hdl $ C.unpack (encode $ Player n "" w l) 

makeMatch :: VConnection -> VConnection -> AcidState UserDB -> IO ()
makeMatch (p1, h1, Just c1) (p2, h2, Just c2) db =
         do g1 <- newStdGen
            g2 <- newStdGen
            g3 <- newStdGen
            let gp1  = mkGamePlayer g1 p1 c1
                gp2  = mkGamePlayer g2 p2 c2
                game = mkGameState  g3 gp1 gp2
            --- |     ALL GAME LOGIC HAPPENS HERE     | ---
            p1in  <- newEmptyMVar
            p2in  <- newEmptyMVar
            error <- newEmptyMVar
            comm  <- newEmptyMVar
            thr1 <- forkIO $ fix $ \loop -> do res <- tryIOError ((C.pack . init) <$> (hGetLine h1) >>= tryPutMVar p1in)
                                               case res of
                                                Left _  -> handleBrokenPipe h2 >> putMVar error True
                                                Right _ -> loop
            thr2 <- forkIO $ fix $ \loop -> do res <- tryIOError ((C.pack . init) <$> (hGetLine h2) >>= tryPutMVar p2in)
                                               case res of
                                                Left _  -> handleBrokenPipe h1 >> putMVar error True
                                                Right _ -> loop
            gthrd <- forkIO $ fix (\loop g -> 
                   case gstate g of
                     P1Win -> undefined
                     P2Win -> undefined
                     Running -> do synchGame h1 h2 g
                                   putStrLn $! if p1turn g then "Player 1 Turn" else "Player 2 Turn"
                                   let (turn, response) = if p1turn g then (p1in,p2in) else (p2in,p1in)
                                   tryTakeMVar turn
                                   tryTakeMVar response
                                   read <- forkIO $ tryTakeMVar response >> takeMVar turn >>= putMVar comm
                                   ctrl <- forkIO $ threadDelay 10000000 >> killThread read 
                                                        >> putMVar comm restAct 
                                   act <- decode' <$> takeMVar comm 
                                   killThread ctrl
                                   putStrLn "End of turn"
                                   case act of 
                                    Nothing -> putMVar error True
                                    Just var -> 
                                          case action var of
                                           Play     -> case card var of 
                                                        Nothing -> putMVar error True
                                                        Just c  -> do g' <- fix (\loop st -> 
                                                                              do synchGame h1 h2 st
                                                                                 return st
                                                                             ) (startStack g c)
                                                                      loop $! nextTurn . resolveStack $ g'
                                           DrawCard -> loop $! nextTurn (drawCard g)
                                           Rest     -> loop $! nextTurn (restPlayer g) ) game 
            end <- takeMVar error
            mapM_ killThread [gthrd, thr1, thr2]
            --- | ----------------------------------- | ---
            --- | ----------------------------------- | ---

restAct = encode $ GameAction Rest Nothing 

startStack (GameState p1 p2 _ g t) i = GameState p1 p2 [i] g t

resolveStack (GameState p1 p2 _ g t) = GameState p1 p2 [] g t

drawCard (GameState p1 p2 s g t) | t         = GameState (draw p1) p2 s (canDraw p1 P2Win) t
                                 | otherwise = GameState p1 (draw p2) s (canDraw p2 P1Win) t
                                 where
                                 canDraw (GamePlayer _ _ (_:_) _ _ _) _ = Running
                                 canDraw (GamePlayer _ _ _     _ _ _) s = s
                                 draw (GamePlayer p h (d:ds) c he s) = GamePlayer p (h++[d]) ds c he s
                                 draw (GamePlayer p h ds     c he s) = GamePlayer p  h       ds c he s

restPlayer (GameState p1 p2 s g t) | t         = GameState (incHealth p1) p2 s g t
                                   | otherwise = GameState p1 (incHealth p2) s g t
                                   where
                                   incHealth gp@(GamePlayer p h d c he s) | he < 30    = GamePlayer p h d c (he+1) s
                                                                          | otherwise = gp

synchGame h1 h2 (GameState p1 p2 s g t) = do let p1' = extract p1
                                                 p2' = extract p2
                                             hPutStrLn h1 . C.unpack . encode $ GameState p1  p2' s g t
                                             hPutStrLn h2 . C.unpack . encode $ GameState p1' p2  s g t
                                        where 
                                        extract (GamePlayer p _ _ c h s) = GamePlayer (strip p) [] [] c h s
                                        strip (Player n _ w l)           = Player n "" w l

handleBrokenPipe hdl =
     do hopen <- hIsWritable hdl
        when hopen (hPutStrLn hdl "{\"error\":\"Partner disconnected -- Game aborted\"}")

mainLoop :: Socket -> MVar Connection ->  IO ()
mainLoop sock mv = 
            do
             conn <- accept sock     
             forkIO $ runConn conn mv
             mainLoop sock mv

runConn :: (Socket, SockAddr) -> MVar Connection -> IO ()
runConn (sock, _) mv = do
                          hdl <- socketToHandle sock ReadWriteMode
                          hSetBuffering hdl NoBuffering
                          line <- fmap (C.pack . init) (hGetLine hdl) 
                          case decode' line :: Maybe ConnPlayer of
                            Just a  -> putMVar mv (a, hdl)
                            Nothing -> hClose hdl 
