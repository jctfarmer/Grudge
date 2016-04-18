{-# LANGUAGE OverloadedStrings #-}
{-# OPTIONS_GHC -fno-warn-unused-do-bind #-}
module Server where

import           Game

import           Control.Applicative
import           Control.Concurrent
import           Control.Concurrent.Chan
import           Control.Concurrent.MVar
import           Control.Monad
import           Control.Monad.Fix
import           Control.Monad.Reader
import           Control.Monad.State
import           Data.Acid
import           Data.Aeson
import           Data.Aeson.Types
import           Data.Array
import qualified Data.ByteString.Lazy.Char8 as C
import qualified Data.Map.Strict            as M
import           Data.Maybe
import           Data.SafeCopy
import           Data.Typeable
import           GHC.Generics
import           Network.Socket
import           System.IO
import           System.IO.Error
import           System.Random

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
                        let chr = chara c
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
                                 do sendStats (p, hdl, chr) >> threadDelay 40000
                                    if queued c && isJust chr
                                       then if not . null $ ps
                                             then do forkIO $ killThread (snd $ last ps)
                                                                >> makeMatch (p, hdl, chr) (fst $ last ps) db
                                                     playQueue (init ps) mv db
                                             else forkIO (pingConn hdl) >>= \t -> playQueue (((p,hdl,chr),t):ps) mv db
                                       else forkIO (threadDelay 40000 >> hClose hdl) >> playQueue ps mv db

type VConnection = (Player, Handle, Maybe Character)

sendStats :: VConnection -> IO ()
sendStats (Player n _ w l, hdl, _) = hPutStrLn hdl (C.unpack (encode $ Player n "" w l))

makeMatch :: VConnection -> VConnection -> AcidState UserDB -> IO ()
makeMatch (p1', h1, Just c1) (p2', h2, Just c2) db =
         do [g1, g2, g3] <- replicateM 3 newStdGen
            let gp1  = mkGamePlayer g1 p1' c1
                gp2  = mkGamePlayer g2 p2' c2
                game = mkGameState  g3 gp1 gp2
            --- |     ALL GAME LOGIC HAPPENS HERE     | ---
            [p1in, p2in, comm] <- replicateM 3 newEmptyMVar
            error <- newEmptyMVar
            thr1 <- forkIO $ fix $ \loop -> do res <- tryIOError (C.pack . init <$> hGetLine h1 >>= tryPutMVar p1in)
                                               case res of
                                                Left _  -> putMVar error True
                                                Right _ -> loop
            thr2 <- forkIO $ fix $ \loop -> do res <- tryIOError (C.pack . init <$> hGetLine h2 >>= tryPutMVar p2in)
                                               case res of
                                                Left _  -> putMVar error True
                                                Right _ -> loop
            gthrd <- forkIO $ fix (\loop g ->
                   case gstate g of
                     Running -> do synchGame h1 h2 g
                                   putStrLn $! if p1turn g then "Player 1 Turn" else "Player 2 Turn"
                                   let (turn, response) = if p1turn g then (p1in,p2in) else (p2in,p1in)
                                   tryTakeMVar comm
                                   tryTakeMVar turn
                                   tryTakeMVar response
                                   read <- forkIO $ tryTakeMVar response >> takeMVar turn >>= putMVar comm
                                   ctrl <- forkIO $ threadDelay 10000000 >> killThread read
                                                        >> putMVar comm restAct
                                   act <- takeMVar comm
                                   print act
                                   killThread ctrl
                                   putStrLn "End of turn"
                                   case decode' act of
                                    Nothing -> putMVar error True
                                    Just var ->
                                          case action var of
                                           Play     -> case card var of
                                                        Nothing -> putMVar error True
                                                        Just c  -> if validateCard c g
                                                                    then
                                                                     do g' <- fix (\loop st tn resp ->
                                                                                do synchGame h1 h2 st
                                                                                   return st
                                                                               ) (startStack g c) turn response
                                                                        loop $! nextTurn . resolveStack $ g'
                                                                    else
                                                                     loop $! drawCard $ nextTurn g
                                           Rest     -> loop $! drawCard . nextTurn $ restPlayer g
                                           Stand    -> loop $! drawCard . nextTurn $ if p1turn g
                                                                                        then g { p1 = standup $ p1 g }
                                                                                        else g { p2 = standup $ p2 g }
                                           _        -> putMVar error True
                     P1Win       -> putMVar error False
                     P2Win       -> putMVar error False ) game

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
                                             inhand = c `elem` hand (if t then p1 else p2)
                                         in inhand && (Counter `notElem` eff)
                                                && ((MustBeDown `notElem` eff) || stand == Down)

restAct = encode $ GameAction Rest Nothing

dropFirst a [] = []
dropFirst a (x:xs) | x == a    = xs
                   | otherwise = x : dropFirst a xs

startStack g i = let t = p1turn g
                 in g { p1 = if t then (p1 g) { hand = dropFirst i . hand $ p1 g } else p1 g
                      , p2 = if t then p2 g else (p2 g) { hand = dropFirst i . hand $ p2 g }
                      , stack = [i] }

doEffects won id (GameState p1 p2 s g t) = let (plyr,opp) = if t then (p1,p2) else (p2,p1)
                                               target = foldr runtEffect (if won then opp else plyr) effects
                                               self   = foldr runsEffect (if won then plyr else opp) effects
                                               p1'    = if (t && won) || (not t && not won) then self else target
                                               p2'    = if (t && won) || (not t && not won) then target else self
                                               effects = getEffects $ theCards ! id
                                           in GameState p1' p2' s g (if Skip `elem` effects then not t else t)
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

canDraw (GamePlayer _ _ (_:_) _ _ _ _) _ = Running
canDraw GamePlayer {}                s = s
draw (GamePlayer p h (d:ds) c he t s) = GamePlayer p (drop (length h-4) $ h++[d]) ds c he t s
draw (GamePlayer p h ds     c he t s) = GamePlayer p  h       ds c he t s

restPlayer (GameState p1 p2 s g t) | t         = GameState (incHealth p1) p2 s g t
                                   | otherwise = GameState p1 (incHealth p2) s g t
                                   where
                                   incHealth p | health p < 30 = p { health = health p + 1}
                                               | otherwise     = p

synchGame h1 h2 (GameState p1 p2 s g t) = do let p1' = extract p1
                                                 p2' = extract p2
                                             hPutStrLn h1 . C.unpack . encode $ GameState p1  p2' s g t
                                             hPutStrLn h2 . C.unpack . encode $ GameState p1' p2  s g t
                                        where
                                        extract (GamePlayer p _ _ c h t s) = GamePlayer (strip p) [] [] c h t s
                                        strip (Player n _ w l)           = Player n "" w l

mainLoop :: Socket -> MVar Connection ->  IO ()
mainLoop sock mv = fix $ \loop ->
                       do
                        conn <- accept sock
                        forkIO $ runConn conn mv
                        loop

runConn :: (Socket, SockAddr) -> MVar Connection -> IO ()
runConn (sock, _) mv = do
                          hdl <- socketToHandle sock ReadWriteMode
                          hSetBuffering hdl NoBuffering
                          void . tryIOError $
                                       do line <- C.pack . init <$> hGetLine hdl
                                          case decode' line of
                                            Just a  -> putMVar mv (a, hdl)
                                            Nothing -> hClose hdl
