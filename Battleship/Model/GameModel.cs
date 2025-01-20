using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Pattern;

namespace Battleship.Model
{
    public class GameModel : EventObserver
    {
        private IPlayer _player1;
        public IPlayer player1
        {
            get { return _player1; }
            set
            {
                _player1 = value;
                CurrentPlayer = value;
            }
        }
        public IPlayer player2;
        public IPlayer CurrentPlayer { get; private set; }
        public bool GameOver { get; private set; }
        private IPlayer winner;
        public IPlayer Winner { get; private set; }
        public string gameMode;

        public GameModel()
        {
            GameOver = false;
        }

        public ShipFleet getShipFleet(IPlayer player)
        {
            return player == player1 ? player1.GetFleet() : player2.GetFleet();
        }

        public List<Shot> getShots(IPlayer player)
        {
            return player == player1 ? player1.GetShots() : player2.GetShots();
        }

        public Ship? getCurrentPlaceholderShip()
        {
            return CurrentPlayer.CurrentPlaceholderShip;
        }

        public int addShot(IPlayer player)
        {
            int result = player.AddShot(player == player1 ? player2.GetFleet() : player1.GetFleet());

            if (result == 0)
            {
                ShipFleet targetFleet = player == player1 ? player2.GetFleet() : player1.GetFleet();
                if (targetFleet.IsSunk())
                {
                    GameOver = true;
                    Winner = player;
                    SaveGameHistory(player);
                    Notify("GAME_OVER");
                }
            }

            return result;
        }

        public void resetGame()
        {
            GameOver = false;
            CurrentPlayer = player1;
            Winner = null;
            gameMode = "";
        }

        private void SaveGameHistory(IPlayer winner)
        {
            var gameRecord = new GameRecord
            {
                Player1Name = player1.name,
                Player2Name = player2.name,
                Player1Shots = player1.GetShots().Count,
                Player2Shots = player2.GetShots().Count,
                Winner = winner.name,
                GameDate = DateTime.Now
            };
            GameHistory.SaveGame(gameRecord);
        }

        public void nextTurn()
        {
            CurrentPlayer = CurrentPlayer == player1 ? player2 : player1;
        }
    }
}
