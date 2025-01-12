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
        Vector2i position = new Vector2i(0, 0);
                    Orientation orientation = Orientation.Horizontal;

        public GameModel()
        {
            GameOver = false;
            shipFleets.Add(new ShipFleet());
            shipFleets.Add(new ShipFleet());
            shots.Add(new List<Shot>());
            shots.Add(new List<Shot>());

            // Przykładowe strzały
            /*
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(0, 0)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(7, 9)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(5, 4)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(1, 5)));
            shots[(int)Player.Player1].Add(new Shot(new Vector2i(3, 2)));
            */

        }

        public Ship addShipToFleet(Player player, int option)
        {
            int length = 0;
            Orientation orientation = Orientation.Horizontal; // Default orientation
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

            Console.WriteLine("Enter the ship's position (e.g., B2):");
            Console.WriteLine("Press 'r' to rotate the ship (before entering position)");

            // Listen for rotation key before getting position
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.R)
                    {
                        orientation = orientation == Orientation.Horizontal ?
                            Orientation.Vertical : Orientation.Horizontal;
                        Console.WriteLine($"Ship orientation changed to: {orientation}");
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            }

            Console.WriteLine("Enter the ship's position (e.g., B2):");
            string input = Console.ReadLine();

            // Validate the input
            if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
            {
                Console.WriteLine("Invalid input.");
                return null;
            }

            // Split the input into a letter and a number
            char column = input[0];
            if (!char.IsLetter(column))
            {
                Console.WriteLine("Invalid column. Must be a letter.");
                return null;
            }

            if (!int.TryParse(input.Substring(1), out int row))
            {
                Console.WriteLine("Invalid row. Must be a number.");
                return null;
            }

            // Convert the input into a Vector2i coordinate
            Vector2i pos1 = new Vector2i(column - 'A', row - 1);

            Ship ship = shipBuilder
                .SetLength(length)
                .SetOrientation(orientation)
                .SetStartPosition(pos1)
                .Build();

            // Add the ship to the fleet
            shipFleets[(int)player].addShip(ship);
            currentShipCount++;
            if (currentShipCount == SHIP_COUNT)
            {
                Console.WriteLine("All ships placed for this player.");
                return null;
            }
            return ship;
        }

        public Ship choosePlacement(Player player, int option)
        {
            int length = 0;
            switch (option)
            {
                case 1: length = 5; break;
                case 2: length = 4; break;
                case 3: length = 3; break;
                default: return null;
            }

            // ship placeholder
            Ship placeholderShip = shipBuilder
                .SetLength(length)
                .SetOrientation(orientation)
                .SetStartPosition(position)
                .Build();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // movement and rotation
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        if (position.y > 0)
                            position.y--;
                        break;
                    case ConsoleKey.S:
                        if (orientation == Orientation.Vertical)
                        {
                            if (position.y < 10 - length)
                                position.y++;
                        }
                        else
                        {
                            if (position.y < 9)
                                position.y++;
                        }
                        break;
                    case ConsoleKey.A:
                        if (position.x > 0)
                            position.x--;
                        break;
                    case ConsoleKey.D:
                        if (orientation == Orientation.Horizontal)
                        {
                            if (position.x < 10 - length)
                                position.x++;
                        }
                        else
                        {
                            if (position.x < 9)
                                position.x++;
                        }
                        break;
                    case ConsoleKey.R:
                        orientation = orientation == Orientation.Horizontal ?
                            Orientation.Vertical : Orientation.Horizontal;
                        break;
                    case ConsoleKey.Enter: // trying to create a ship at the current position

                        Ship finalShip = shipBuilder
                            .SetLength(length)
                            .SetOrientation(orientation)
                            .SetStartPosition(position)
                            .Build();

                        // add the ship to the fleet if it's a valid placement
                        if (IsValidPlacement(finalShip, player))
                        {
                            shipFleets[(int)player].addShip(finalShip);
                            position.x = 0;
                            position.y = 0;
                            return null;
                        }
                        return finalShip;
                }

                // update placeholder ship position
                placeholderShip = shipBuilder
                    .SetLength(length)
                    .SetOrientation(orientation)
                    .SetStartPosition(position)
                    .Build();

                return placeholderShip;
            }
        }

        private bool IsValidPlacement(Ship ship, Player player)
        {
            Vector2i startPos = ship.getParts()[0].getPosition();
            int length = ship.getParts().Count;

            // check bounds
            if (!IsWithinBounds(startPos, length, orientation))
                return false;

            // check for collisions with other ships
            foreach (ShipPart newPart in ship.getParts())
            {
                foreach (Ship existingShip in shipFleets[(int)player].GetShips())
                {
                    foreach (ShipPart existingPart in existingShip.getParts())
                    {
                        if (newPart.getPosition().x == existingPart.getPosition().x &&
                            newPart.getPosition().y == existingPart.getPosition().y)
                            return false;
                    }
                }
            }
            return true;
        }

        private bool IsWithinBounds(Vector2i position, int length, Orientation orientation)
        {
            if (position.x < 0 || position.y < 0)
                return false;

            // check length-dependent bounds
            if (orientation == Orientation.Horizontal) // hotizontal
            {
                if (position.x + length > 10)
                    return false;
                if (position.y >= 10)
                    return false;
            }
            else // vertical
            {
                if (position.y + length > 10)
                    return false;
                if (position.x >= 10)
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
