{-# LANGUAGE OverloadedStrings, DeriveGeneric, DeriveDataTypeable, TypeFamilies #-}
module Game where

import System.IO
import GHC.Generics
import Data.Aeson
import Data.Aeson.Types
import Data.Typeable

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

data Character = Giant | Luchador | Brawler | MatTechnician | Hero deriving (Show, Generic, Eq) 
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


resolveStack :: GameState -> 
