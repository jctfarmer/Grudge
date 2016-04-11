{-# LANGUAGE OverloadedStrings, DeriveGeneric, DeriveDataTypeable, TemplateHaskell, TypeFamilies #-}
module Server where

import Game

import Data.Acid
import Data.Maybe
import Data.SafeCopy
import Data.Typeable
import Data.Either
import qualified Data.Map.Strict as M
import Control.Concurrent
import Control.Concurrent.MVar
import Control.Concurrent.Chan
import Control.Monad
import Control.Monad.State
import Control.Monad.Reader
import Control.Monad.Fix
import Debug.Trace
import Network.Socket
import GHC.Generics
import Data.Aeson
import Data.Aeson.Types
import qualified Data.ByteString.Lazy.Char8 as C
import System.IO
import System.IO.Error

-- Connections manage game players and their socket handles
type Connection = (ConnPlayer, Handle)

-- Initial Connection
data ConnPlayer = ConnPlayer { username,pass :: String, queued :: Bool } deriving (Show, Generic, Eq)
instance FromJSON ConnPlayer 
instance ToJSON ConnPlayer

newtype UserDB = UserDB { users :: M.Map String Player } deriving (Typeable)

validatePlayer :: ConnPlayer -> Query UserDB (Maybe Player)
validatePlayer (ConnPlayer n p _) = asks $ (\db -> 
                                            case M.lookup n db of
                                               Nothing -> Nothing
                                               Just u  -> if pw u == p 
                                                           then Just u
                                                           else Nothing) . users 

exists :: ConnPlayer -> Query UserDB Bool
exists (ConnPlayer n _ _) = asks $ isNothing . (\db -> M.lookup n db) . users

newPlayer :: ConnPlayer -> Update UserDB ()
newPlayer (ConnPlayer n p _) = modify go
        where
        go (UserDB db) = UserDB $ M.insert n (Player n p 0 0) db

deriveSafeCopy 0 'base ''UserDB
deriveSafeCopy 0 'base ''Player
deriveSafeCopy 0 'base ''ConnPlayer
makeAcidic ''UserDB ['validatePlayer, 'newPlayer, 'exists]

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

playQueue :: [VConnection] -> MVar Connection -> AcidState UserDB -> IO ()
playQueue cs mv db = do (c, hdl) <- takeMVar mv
                        new <- query db (Exists c)
                        when new (putStrLn "New player..." >> update db (NewPlayer c))
                        p <- query db (ValidatePlayer c)
                        print p
                        ps <- filterM (hIsOpen . snd) cs
                        print ps
                        case p of
                           Nothing  -> hPutStrLn hdl "{\"error\":\"Invalid Password\"}" 
                                       >> hClose hdl >> playQueue ps mv db
                           (Just p) -> 
                                 do sendStats (p, hdl)
                                    if queued c 
                                       then if not . null $ ps 
                                             then do forkIO $ makeMatch (p, hdl) (last ps) db
                                                     playQueue (init ps) mv db
                                             else playQueue ((p,hdl):ps) mv db
                                       else playQueue ps mv db

type VConnection = (Player, Handle)

sendStats :: VConnection -> IO ()
sendStats (Player n _ w l, hdl) = hPutStrLn hdl $ C.unpack (encode $ Player n "" w l) 

makeMatch :: VConnection -> VConnection -> AcidState UserDB -> IO ()
makeMatch (p1, h1) (p2, h2) db =
        catchIOError 
         (fix $ \loop -> 
            do hPutStrLn h2 "PING"
               hPutStrLn h1 "PING"
               threadDelay 650000
               loop) 
         (\_ -> do h1open <- hIsOpen h1
                   h2open <- hIsOpen h2
                   when h1open (hClose h1)
                   when h2open (hClose h2))


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
