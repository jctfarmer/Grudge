{-# LANGUAGE OverloadedStrings, DeriveGeneric, DeriveDataTypeable, TypeFamilies, TemplateHaskell #-}
module Game where

import Control.Monad.Reader
import Control.Monad.State
import Data.Acid
import Data.Maybe
import Data.SafeCopy
import Data.Typeable
import Data.Either
import qualified Data.Map.Strict as M
import Network.Socket
import GHC.Generics
import Data.Aeson
import Data.Aeson.Types
import qualified Data.ByteString.Lazy.Char8 as C
import System.IO
import System.IO.Error

data Action     = Play | Wait | Stack deriving (Show, Generic, Eq)
instance FromJSON Action 
instance ToJSON Action

data GameAction = GameAction { action :: Action
                             , card   :: Maybe Int } deriving (Show, Generic)
instance FromJSON GameAction 
instance ToJSON GameAction

data CardType = Pin | Submit | PCounter | PAttack | Counter | CAttack | Escape | EPin | Attack | Special deriving (Show, Eq, Generic)
instance FromJSON CardType 
instance ToJSON CardType

data Stance   = Up | Kneel | Down deriving (Eq, Show, Generic)
instance FromJSON Stance 
instance ToJSON Stance

data Game     = Running | P1Win | P2Win deriving (Show, Eq, Generic)
instance FromJSON Game
instance ToJSON Game

data Character = Giant | Luchador | Brawler | Technician | Hero deriving (Show, Generic, Eq) 
instance FromJSON Character
instance ToJSON Character

data Player = Player { 
                name,pw     :: String
              , wins,losses :: Int } deriving (Show, Eq, Generic, Typeable)
instance FromJSON Player
instance ToJSON Player

data GamePlayer = GamePlayer {
                player    :: Player
              , hand      :: [Card]
              , deck      :: [Card]
              , character :: Character
              , health    :: Int
              , stance    :: Stance } deriving (Show, Eq, Generic)
instance FromJSON GamePlayer
instance ToJSON GamePlayer

data GameState = GameState {
                p1, p2  :: GamePlayer
              , stack   :: [Card]
              , gstate  :: Game
              , p1turn  :: Bool } deriving (Show, Eq, Generic)
instance FromJSON GameState
instance ToJSON GameState
                    
data Card = Card {
                ctype         :: CardType
              , attack,cardid :: Int } deriving (Show, Eq, Generic)
instance FromJSON Card
instance ToJSON Card


-- resolveStack :: GameState -> 

-- Initial Connection
data ConnPlayer = ConnPlayer { username,pass :: String, queued :: Bool, char :: Maybe Character } deriving (Show, Generic, Eq)
instance FromJSON ConnPlayer 
instance ToJSON ConnPlayer

newtype UserDB = UserDB { users :: M.Map String Player } deriving (Typeable)

validatePlayer :: ConnPlayer -> Query UserDB (Maybe Player)
validatePlayer (ConnPlayer n p _ _) = asks $ (\db -> 
                                            case M.lookup n db of
                                               Nothing -> Nothing
                                               Just u  -> if pw u == p 
                                                           then Just u
                                                           else Nothing) . users 

exists :: ConnPlayer -> Query UserDB Bool
exists (ConnPlayer n _ _ _) = asks $ isNothing . (\db -> M.lookup n db) . users

newPlayer :: ConnPlayer -> Update UserDB ()
newPlayer (ConnPlayer n p _ _) = modify go
        where
        go (UserDB db) = UserDB $ M.insert n (Player n p 0 0) db

deriveSafeCopy 0 'base ''UserDB
deriveSafeCopy 0 'base ''Player
deriveSafeCopy 0 'base ''ConnPlayer
deriveSafeCopy 0 'base ''Character
makeAcidic ''UserDB ['validatePlayer, 'newPlayer, 'exists]
