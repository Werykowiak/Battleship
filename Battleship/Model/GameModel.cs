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
            string? coordinates = Console.ReadLine();
            if (string.IsNullOrEmpty(coordinates) || coordinates.Length < 2)
            {
                return 1;
            }

            if (!char.IsLetter(coordinates[0]) || coordinates[0] < 'A' || coordinates[0] > 'J')
            {
                return 2;
            }

            string numberPart = coordinates.Substring(1);
            if (!int.TryParse(numberPart, out int number) || number < 1 || number > 10)
            {
                return 3;
            }

            Vector2i pos = new Vector2i(coordinates[0] - 'A', number - 1);

            if (player == player1)
            {
                player1.AddShot(pos);

                List<ShipPart> parts = player2.GetFleet().getParts();
                foreach (ShipPart part in parts)
                {
                    if (part.Shoot(pos.x, pos.y))
                    {
                        return 0;
                    }
                }
            }
            else
            {
                player2.AddShot(pos);

                List<ShipPart> parts = player1.GetFleet().getParts();
                foreach (ShipPart part in parts)
                {
                    if (part.Shoot(pos.x, pos.y))
                    {
                        return 0;
                    }
                }
            }

            return 4;
        }

        public void nextTurn()
        {
            CurrentPlayer = CurrentPlayer == player1 ? player2 : player1;
        }
    }
}
