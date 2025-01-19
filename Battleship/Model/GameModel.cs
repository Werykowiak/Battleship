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
        public IPlayer player1 = new Player("Pierwszy");
        public IPlayer player2 = new AIPlayer("Drugi");
        public IPlayer CurrentPlayer { get; private set; }
        public bool GameOver { get; private set; }
        private IPlayer winner;

        public GameModel()
        {
            GameOver = false;
            CurrentPlayer = player1;
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
                    SaveGameHistory(player);
                    Notify("GAME_OVER");
                }
            }

            return result;
        }

        private void SaveGameHistory(IPlayer winner)
        {
            var gameRecord = new GameRecord
            {
                Player1Name = player1.name,
                Player2Name = player2.name,
                Player1Shots = player1.GetShots().Count,
                Player2Shots = player2.GetShots().Count,
                Winner = winner.name
            };
            GameHistory.SaveGame(gameRecord);
        }

        public void nextTurn()
        {
            CurrentPlayer = CurrentPlayer == player1 ? player2 : player1;
        }
    }
}
