{-# LANGUAGE OverloadedStrings, DeriveGeneric, DeriveDataTypeable, TypeFamilies, TemplateHaskell #-}
module Game where

import Control.Monad.Reader
import Control.Monad.State
import Data.Acid
import Data.Array
import Data.Maybe
import Data.SafeCopy
import Data.Typeable
import Data.Either
import qualified Data.Map.Strict as M
import Network.Socket
import GHC.Generics
import Data.Aeson (fromJSON, toJSON, FromJSON(..), ToJSON(..))
import qualified Data.ByteString.Lazy.Char8 as C
import System.IO
import System.Random
import System.IO.Error

data Action     = Play | Rest | DrawCard | TapDown deriving (Show, Generic, Eq)
instance FromJSON Action 
instance ToJSON Action

data GameAction = GameAction { action :: Action
                             , card   :: Maybe Int } deriving (Show, Generic)
instance FromJSON GameAction 
instance ToJSON GameAction

data CardEffect = Pin 
                | Submit 
                | Skip
                | Draw Int
                | KnockDown
                | Counter 
                | Escape 
                | Attack Int
                | MustBeDown
                | StandUp
                | Special deriving (Show, Eq, Generic)
instance FromJSON CardEffect 
instance ToJSON CardEffect

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
              , hand      :: [Int]
              , deck      :: [Int]
              , character :: Character
              , health    :: Int
              , stance    :: Stance } deriving (Show, Eq, Generic)
instance FromJSON GamePlayer
instance ToJSON GamePlayer

data GameState = GameState {
                p1, p2  :: GamePlayer
              , stack   :: [Int]
              , gstate  :: Game
              , p1turn  :: Bool } deriving (Show, Eq, Generic)
instance FromJSON GameState
instance ToJSON GameState
                    
data Card = Card {
                  getEffects   :: [CardEffect]
                 } deriving (Show, Eq, Generic)
instance FromJSON Card
instance ToJSON Card

type Cards = Array Int Card

theCards :: Cards
theCards = listArray (0,106) 
            $! fmap Card 
            [
                [Attack 5, KnockDown]
            ,   [Attack 2, KnockDown]
            ,   [Counter, Submit]
            ,   [Attack 1]
            ,   [Attack 3, KnockDown]
            ,   [Attack 3, KnockDown]
            ,   [Attack 3, KnockDown]
            ,   [Attack 2, KnockDown]
            ,   [Submit]
            ,   [Attack 2, KnockDown]
            ,   [Attack 2, KnockDown]
            ,   [Attack 3, MustBeDown]
            ,   [Attack 1, Skip]
            ,   [Attack 1]
            ,   [Attack 2, KnockDown]
            ,   [Counter, Skip]
            ,   [Counter, Draw 2]
            ,   [Counter, Draw 1]
            ,   [Counter, Attack 3, KnockDown]
            ,   [Attack 4, KnockDown]
            ,   [Counter, KnockDown]
            ,   [Counter, Draw (-1)]
            ,   [Attack 1, KnockDown]
            ,   [Counter]
            ,   [Counter, Attack 1]
            ,   [Pin]
            ,   [Counter, Draw (-1)]
            ,   [Attack 4, MustBeDown]
            ,   [Attack 2, Skip]
            ,   [Attack 1, KnockDown]
            ,   [Counter, KnockDown]
            ,   [Counter, Skip]
            ,   [Counter, StandUp]
            ,   [Attack 4, KnockDown]
            ,   [Escape]
            ,   [Attack 2]
            ,   [Attack 1, Skip]
            ,   [Attack 2, Pin, KnockDown]
            ,   [Attack 5, KnockDown]
            ,   [Submit]
            ,   [Attack 2, Counter]
            ,   [Attack 2]
            ,   [Attack 2, KnockDown, Pin]
            ,   [Attack 2]
            ,   [Attack 1, Counter]
            ,   [Attack 1, KnockDown]
            ,   [Attack 5, KnockDown]
            ,   [Attack 2, Pin, KnockDown]
            ,   [Counter, Draw 1]
            ,   [Counter, StandUp]
            ,   [Attack 3, MustBeDown]
            ,   [Attack 2, Skip]
            ,   [Attack 2, MustBeDown]
            ,   [Pin, MustBeDown]
            ,   [Counter, Draw 1]
            ,   [Attack 2, MustBeDown]
            ,   [Attack 1]
            ,   [Attack 0, KnockDown]
            ,   [Attack 2, Pin, KnockDown]
            ,   [Attack 4, MustBeDown]
            ,   [Counter, Draw 2]
            ,   [Counter, Draw 1]
            ,   [Counter, StandUp]
            ,   [Attack 3, KnockDown]
            ,   [Pin, MustBeDown]
            ,   [Attack 5, MustBeDown]
            ,   [Attack 3, Counter, KnockDown]
            ,   [Attack 1]
            ,   [Attack 0, KnockDown]
            ,   [Counter, Draw (-1)]
            ,   [Counter, Draw 1]
            ,   [Escape, Pin]
            ,   [Counter, Attack 2]
            ,   [Counter, Attack 3, KnockDown]
            ,   [Attack 4, MustBeDown]
            ,   [Attack 3, MustBeDown]
            ,   [Pin]
            ,   [Submit]
            ,   [Submit]
            ,   [Pin, MustBeDown]
            ,   [Attack 2]
            ,   [Counter, KnockDown]
            ,   [Attack 2]
            ,   [Attack 2, Skip]
            ,   [Submit]
            ,   [Counter, Pin]
            ,   [Attack 0, KnockDown]
            ,   [Attack 2, Counter]
            ,   [Pin]
            ,   [Attack 3, MustBeDown]
            ,   [Attack 3, MustBeDown]
            ,   [Submit]
            ,   [Attack 2, MustBeDown]
            ,   [Counter, Draw 2]
            ,   [Attack 4, KnockDown]
            ,   [Attack 4, KnockDown]
            ,   [Attack 4, KnockDown]
            ,   [Counter, Draw 2]
            ,   [Counter, KnockDown, Attack 2]
            ,   [Attack 0, Skip]
            ,   [Attack 2, KnockDown]
            ,   [Attack 4, MustBeDown]
            ,   [Attack 2, Counter]
            ,   [Attack 3, KnockDown]
            ,   [Pin]
            ,   [Counter, Draw 2]
            ,   [Attack 2, MustBeDown]
                
            ]
mkGameState :: StdGen -> GamePlayer -> GamePlayer -> GameState
mkGameState g p1 p2 = GameState p1 p2 [] Running (fst $ randomR (True, False) g)

mkGamePlayer :: StdGen -> Player -> Character -> GamePlayer
mkGamePlayer g p c = let (hand, deck) = splitAt 5 . take 70 $ (randomRs (0,106) g :: [Int])
                     in GamePlayer p hand deck c 25 Up

nextTurn :: GameState -> GameState
nextTurn (GameState p1 p2 s r t) = GameState p1 p2 s r (not t)

-- resolveStack :: GameState -> 

-- Initial Connection
data ConnPlayer = ConnPlayer { username,pass :: String, queued :: Bool, chara :: Maybe Character } deriving (Show, Generic, Eq)
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
