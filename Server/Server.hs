{-# LANGUAGE OverloadedStrings #-}
module Server where

import Game

import Data.Acid
import Data.Array
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

pingConn hdl = fix $ \loop -> do 
                            res <- tryIOError (hPutStrLn hdl "PING" >> threadDelay 400000)
                            case res of
                                Left _ -> hClose hdl
                                Right _ -> loop

playQueue :: [(VConnection,ThreadId)] -> MVar Connection -> AcidState UserDB -> IO ()
playQueue cs mv db = do (c, hdl) <- takeMVar mv
                        let chr = char c
                        new <- query db (Exists c)
                        when new (putStrLn "New player..." >> update db (NewPlayer c))
                        p <- query db (ValidatePlayer c)
                        print p
                        ps <- filterM (hIsOpen . snd' . fst) cs
                        print ps
                        case p of
                           Nothing  -> hPutStrLn hdl "{\"error\":\"Invalid Password\"}" 
                                       >> hClose hdl >> playQueue ps mv db
                           (Just p) -> 
                                 do sendStats (p, hdl, chr)
                                    if queued c && isJust chr
                                       then if not . null $ ps 
                                             then do forkIO $ killThread (snd $ last ps) 
                                                                >> makeMatch (p, hdl, chr) (fst $ last ps) db
                                                     playQueue (init ps) mv db
                                             else forkIO (pingConn hdl) >>= \t -> playQueue (((p,hdl,chr),t):ps) mv db
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
                                                Left _  -> putMVar error True
                                                Right _ -> loop
            thr2 <- forkIO $ fix $ \loop -> do res <- tryIOError ((C.pack . init) <$> (hGetLine h2) >>= tryPutMVar p2in)
                                               case res of
                                                Left _  -> putMVar error True
                                                Right _ -> loop
            gthrd <- forkIO $ fix (\loop g -> 
                   case gstate g of
                     P1Win -> undefined
                     P2Win -> undefined
                     Running -> do synchGame h1 h2 g
                                   putStrLn $! if p1turn g then "Player 1 Turn" else "Player 2 Turn"
                                   let (turn, response) = if p1turn g then (p1in,p2in) else (p2in,p1in)
                                   tryTakeMVar comm
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
                                                        Just c  -> if validateCard c g
                                                                    then
                                                                     do g' <- fix (\loop st -> 
                                                                                do synchGame h1 h2 st
                                                                                   return st
                                                                               ) (startStack g c)
                                                                        loop $! nextTurn . resolveStack $ g'
                                                                    else
                                                                     loop $! nextTurn g
                                           DrawCard -> loop $! nextTurn (drawCard g)
                                           Rest     -> loop $! nextTurn (restPlayer g) ) game 
            end <- takeMVar error
            putStrLn "Game Ended"
            mapM_ killThread [gthrd, thr1, thr2]
            when end (handleBrokenPipe h1 h2)
            --- | ----------------------------------- | ---
            --- | ----------------------------------- | ---

handleBrokenPipe h1 h2 =
     do h1open <- hIsOpen h1
        h2open <- hIsOpen h2
        tryIOError $ hPutStrLn h1 "{\"error\":\"Partner disconnected -- Game aborted\"}"
        tryIOError $ hPutStrLn h2 "{\"error\":\"Partner disconnected -- Game aborted\"}"
        when h1open (hClose h1)
        when h2open (hClose h2)

validateCard c (GameState p1 p2 s g t) = let eff   = getEffects $ theCards ! c
                                             stand = stance $ if t then p1 else p2
                                         in (Counter `notElem` eff) 
                                                && ((MustBeDown `notElem` eff) || stand == Down)

restAct = encode $ GameAction Rest Nothing 

startStack (GameState p1 p2 _ g t) i = GameState p1 p2 [i] g t

doEffects won id (GameState p1 p2 s g t) = let (plyr,opp) = if t then (p1,p2) else (p2,p1)
                                               target = foldr runtEffect (if won then opp else plyr) effects
                                               self   = foldr runsEffect (if won then plyr else opp) effects
                                               p1'    = if (t && won) || (not t && not won) then self else target
                                               p2'    = if (t && won) || (not t && not won) then target else self
                                               effects = getEffects $ theCards ! id
                                           in undefined
                                         where
                                         runsEffect e p = case e of
                                                          Draw   n  -> if n > 0 then foldl (\p _ -> draw p) p [1..n] else p
                                                          StandUp   -> standup p
                                                          _         -> p
                                         runtEffect e p = case e of
                                                          Attack n  -> p { health = health p - n }       
                                                          Draw   n  -> if n < 0 then p { hand = drop (abs n) $ hand p } else p
                                                          KnockDown -> knock p
                                                          _         -> p

standup p | stance p == Down && character p /= Giant   = p { stance = Up }
          | stance p == Down && character p == Giant   = p { stance = Kneel }
          | stance p == Kneel                          = p { stance = Up }
          | otherwise                                  = p

knock p | stance p == Up && character p /= Giant  = p { stance = Down }
        | stance p == Up && character p == Giant  = p { stance = Kneel }
        | stance p == Kneel                          = p { stance = Down }
        | otherwise                                  = p

resolveStack (GameState p1 p2 s g t) = let game = GameState p1 p2 [] g t
                                           win = odd . length $ s 
                                       in foldr (doEffects win) game $ processStack s
                                     where
                                     processStack [l]      = [l]
                                     processStack (l:r:xs) = l : processStack xs

drawCard (GameState p1 p2 s g t) | t         = GameState (draw p1) p2 s (canDraw p1 P2Win) t
                                 | otherwise = GameState p1 (draw p2) s (canDraw p2 P1Win) t

canDraw (GamePlayer _ _ (_:_) _ _ _) _ = Running
canDraw (GamePlayer _ _ _     _ _ _) s = s
draw (GamePlayer p h (d:ds) c he s) = GamePlayer p (drop (length h-4) $ h++[d]) ds c he s
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
