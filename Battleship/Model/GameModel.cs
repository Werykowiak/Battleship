using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Pattern;

namespace Battleship.Model
{
    internal class GameModel : EventObserver
    {
        private bool GameOver { get;  set; }
        private const int BOARD_SIZE = 10;
        private const int SHIP_COUNT = 5;
        private int currentShipCount = 0;
        private List<ShipFleet> shipFleets = new List<ShipFleet>(2);
        private List<List<Shot>> shots = new List<List<Shot>>(2);
        private ShipBuilder builder = new ShipBuilder();

        public GameModel()
        {
            GameOver = false;
            shipFleets.Add(new ShipFleet());
            shipFleets.Add(new ShipFleet());
            shots.Add(new List<Shot>());
            shots.Add(new List<Shot>());

            // Przykładowe strzały
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(0, 0)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(7, 9)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(5, 4)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(1, 5)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(3, 2)));

        }

        public bool addShipToFleet(Player player)
        {
            // Kod który pobiera wejście w formacie A1:A4 lub B2:D2 i przekonwertuj je na współrzędne
            string coordinates = Console.ReadLine();
            string[] coordinatesArray = coordinates.Split(':');

            if (coordinatesArray.Length != 2)
            {
                Console.WriteLine("Invalid input.");
                return false;
            }

            Vector2i pos1 = new Vector2i(coordinatesArray[0][0] - 'A', coordinatesArray[0][1] - '1');
            Vector2i pos2 = new Vector2i(coordinatesArray[1][0] - 'A', coordinatesArray[1][1] - '1');

            if (builder.AddShip(pos1, pos2, shipFleets[(int)player]) == false)
            {
                //Console.WriteLine("Ship cannot be placed here.");
                return true;
            }

            shipFleets[(int)player].addShip(builder.Build());
            //Console.WriteLine("Starting position: " + pos1.x + " " + pos1.y);
            //Console.WriteLine("Ending position: " + pos2.x + " " + pos2.y);

            currentShipCount++;

            if (currentShipCount == SHIP_COUNT)
                return false;
            return true;
        }

        public ShipFleet getShipFleet(Player player)
        {
            return shipFleets[(int)player];
        }

        public List<Shot> getShots(Player player)
        {
            return shots[(int)player];
        }

        public void addShot(Player player)
        {
            // Kod który pobiera wejście w formacie A1 lub B2 i przekonwertuj je na współrzędne
            string? coordinates = Console.ReadLine();
            if (string.IsNullOrEmpty(coordinates))
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            Vector2i pos = new Vector2i(coordinates[0] - 'A', coordinates[1] - '1');
            Shot shot = new Shot(pos);
            shots[(int)player].Add(shot);
        }
    }
}
