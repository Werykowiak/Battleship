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
        private ShipBuilder shipBuilder = new ShipBuilder();

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


        public bool addShipToFleet(Player player, int option)
        {
            int length = 0;
            switch (option)
            {
                case 1:
                    length = 5;
                    break;
                case 2:
                    length = 4;
                    break;
                case 3:
                    length = 3;
                    break;
            }

            // Get the input in the format B2, C3, or F5
            Console.WriteLine("Enter the ship's position (e.g., B2):");
            string input = Console.ReadLine();

            // Validate the input
            if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
            {
                Console.WriteLine("Invalid input.");
                return false;
            }

            // Split the input into a letter and a number
            char column = input[0];
            if (!char.IsLetter(column))
            {
                Console.WriteLine("Invalid column. Must be a letter.");
                return false;
            }

            if (!int.TryParse(input.Substring(1), out int row))
            {
                Console.WriteLine("Invalid row. Must be a number.");
                return false;
            }

            // Convert the input into a Vector2i coordinate
            Vector2i pos1 = new Vector2i(column - 'A', row - 1);

            // Attempt to add the ship using the builder
            Ship ship = shipBuilder
                .SetLength(length)
                .SetOrientation(Orientation.Horizontal)
                .SetStartPosition(pos1)
                .Build();

            // Add the ship to the fleet
            shipFleets[(int)player].addShip(ship);
            currentShipCount++;

            if (currentShipCount == SHIP_COUNT)
            {
                Console.WriteLine("All ships placed for this player.");
                return false;
            }

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
