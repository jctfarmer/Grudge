{-# LANGUAGE DeriveDataTypeable #-}
{-# LANGUAGE DeriveGeneric      #-}
{-# LANGUAGE OverloadedStrings  #-}
{-# LANGUAGE TemplateHaskell    #-}
{-# LANGUAGE TypeFamilies       #-}
{-# OPTIONS_GHC -fno-warn-unused-do-bind #-}
module Game where

import           Control.Lens
import           Control.Monad.Reader
import           Control.Monad.State
import           Data.Acid
import           Data.Aeson           (FromJSON (..), ToJSON (..))
import           Data.Array
import qualified Data.Map.Strict      as M
import           Data.Maybe
import           Data.SafeCopy
import           Data.Typeable
import           GHC.Generics
import           System.Random

data Action     = Play | Rest | Stand | TapDown deriving (Show, Generic, Eq)
instance FromJSON Action
instance ToJSON Action

data GameAction = GameAction { _action :: Action
                             , _card   :: Maybe Int } deriving (Show, Generic)
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
                _name,_pw     :: String
              , _wins,_losses :: Int } deriving (Show, Eq, Generic, Typeable)
instance FromJSON Player
instance ToJSON Player

data GamePlayer = GamePlayer {
                _player    :: Player
              , _hand      :: [Int]
              , _deck      :: [Int]
              , _character :: Character
              , _health    :: Int
              , _tapdown   :: Bool
              , _stance    :: Stance } deriving (Show, Eq, Generic)
instance FromJSON GamePlayer
instance ToJSON GamePlayer

data GameState = GameState {
                _p1, _p2 :: GamePlayer
              , _stack   :: [Int]
              , _gstate  :: Game
              , _p1turn  :: Bool } deriving (Show, Eq, Generic)
instance FromJSON GameState
instance ToJSON GameState

data Card = Card {
                  getEffects :: [CardEffect]
                 } deriving (Show, Eq, Generic)
instance FromJSON Card
instance ToJSON Card

makeLenses ''Player
makeLenses ''GameAction
makeLenses ''GameState
makeLenses ''GamePlayer
makeLenses ''Character

type Cards = Array Int Card

theCards :: Cards
theCards = listArray (0,106)
            $! fmap Card
            [
                [Attack 5, Special, KnockDown]
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
            ,   [Attack 5, Special, KnockDown]
            ,   [Submit]
            ,   [Attack 2, Counter]
            ,   [Attack 2]
            ,   [Attack 2, KnockDown, Pin]
            ,   [Attack 2]
            ,   [Attack 1, Counter]
            ,   [Attack 1, KnockDown]
            ,   [Attack 5, Special, KnockDown]
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
            ,   [Attack 5, Special, MustBeDown]
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
            ,   [Submit, Special]
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
mkGamePlayer g p c = let (hand, deck) = splitAt 4 . take 50 $ (randomRs (0,106) g :: [Int])
                     in GamePlayer p hand deck c 25 False Up

nextTurn :: GameState -> GameState
nextTurn = p1turn %~ not

-- Initial Connection
data ConnPlayer = ConnPlayer { username,pass :: String, queued :: Bool, chara :: Maybe Character } deriving (Show, Generic, Eq)
instance FromJSON ConnPlayer
instance ToJSON ConnPlayer

newtype UserDB = UserDB { users :: M.Map String Player } deriving (Typeable)

validatePlayer :: ConnPlayer -> Query UserDB (Maybe Player)
validatePlayer (ConnPlayer n p _ _) = asks $ (\db ->
                                            case M.lookup n db of
                                               Nothing -> Nothing
                                               Just u  -> if u ^. pw == p
                                                           then Just u
                                                           else Nothing) . users

exists :: ConnPlayer -> Query UserDB Bool
exists (ConnPlayer n _ _ _) = asks $ isNothing . M.lookup n . users

newPlayer :: ConnPlayer -> Update UserDB ()
newPlayer (ConnPlayer n p _ _) = modify go
        where
        go (UserDB db) = UserDB $ M.insert n (Player n p 0 0) db

deriveSafeCopy 0 'base ''UserDB
deriveSafeCopy 0 'base ''Player
deriveSafeCopy 0 'base ''ConnPlayer
deriveSafeCopy 0 'base ''Character
makeAcidic ''UserDB ['validatePlayer, 'newPlayer, 'exists]
