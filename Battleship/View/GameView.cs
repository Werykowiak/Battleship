using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Model;
using Battleship.Pattern;
namespace Battleship.View
{
    internal class GameView : EventObserver
    {
        public void DisplayBuildInstructions(int i)
        {
            switch(i){
                case 1:
                    Console.WriteLine("Choose a ship to place: ");
                    break;
                case 2:
                    Console.WriteLine("Choose where to place it: ");
                    break;
            }
        }

        public void DisplayRemainingShipsToPlace(ShipFleet fleet)
        {
            Console.WriteLine("Remaining ships to place: ");
            Console.WriteLine("1. Remaining 5-length ships: " + (1 - fleet.getShipCount(5)).ToString());
            Console.WriteLine("2. Remaining 4-length ships: " + (2 - fleet.getShipCount(4)).ToString());
            Console.WriteLine("3. Remaining 3-length ships: " + (3 - fleet.getShipCount(3)).ToString());
            Console.WriteLine("");
        }

        public void DisplayMap(List<ShipPart> shipParts, List<Shot> firedShots)
        {
            DrawBaseGrid();
            DisplayShipParts(shipParts);
            
            (int left, int top) = Console.GetCursorPosition();
            
            foreach (Shot shot in firedShots)
            {
                Console.SetCursorPosition(shot.getPosition().x * 5 + 6, shot.getPosition().y * 2 + 1);
                shot.display();
            }
            
            Console.SetCursorPosition(0, 22);
        }

        public void DisplayBuilderMap(List<ShipPart> shipParts, Ship? placeholderShip)
        {
            DrawBaseGrid();
            DisplayShipParts(shipParts);
            
            if (placeholderShip != null)
            {
                DisplayShipParts(placeholderShip.getParts());
            }
        }

        private void DrawBaseGrid()
        {
            Console.WriteLine("      A    B    C    D    E    F    G    H    I    J   ");

            for (int i = 1; i <= 10; i++)
            {
                Console.Write($"{i,2}    ");
                for (int j = 0; j < 10; j++)
                    Console.Write("     ");

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private void DisplayShipParts(List<ShipPart> parts)
        {
            (int left, int top) = Console.GetCursorPosition();
            
            foreach (ShipPart part in parts)
            {
                Console.SetCursorPosition(part.getPosition().x * 5 + 6, part.getPosition().y * 2 + 1);
                part.display();
            }
            
            Console.SetCursorPosition(left, top);
        }

        public void DisplayShotInstructions()
        {
            Console.WriteLine("Enter the coordinates of the shot: ");
        }

        public ConsoleKeyInfo GetInput()
        {
            return Console.ReadKey(true);
        }

        public void DisplayCurrentPlayer(string playerName)
        {
            (int left, int top) = Console.GetCursorPosition();
            
            Console.SetCursorPosition(Console.WindowWidth - playerName.Length - 15, 0);
            Console.Write("Tura gracza: " + playerName);
            
            Console.SetCursorPosition(left, top);
        }

        public void BuildFleetForPlayer(Player player)
        {
            while (!player.GetFleet().isComplete())
            {
                Console.Clear();
                DisplayBuilderMap(player.GetFleet().getParts(), player.CurrentPlaceholderShip);
                DisplayCurrentPlayer(player.name);
                DisplayRemainingShipsToPlace(player.GetFleet());
                DisplayBuildInstructions(1);

                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int shipType) || shipType < 1 || shipType > 3)
                    continue;

                PlaceShipForPlayer(player, shipType);
            }
        }

        private void PlaceShipForPlayer(Player player, int shipType)
        {
            int length = shipType == 1 ? 5 : shipType == 2 ? 4 : 3;
            
            if (!player.CanAddShipOfLength(length))
            {
                Console.WriteLine($"Cannot add more ships of length {length}.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            while (true)
            {
                Console.Clear();
                player.UpdatePlaceholderShip(length);
                DisplayBuilderMap(player.GetFleet().getParts(), player.CurrentPlaceholderShip);
                DisplayCurrentPlayer(player.name);

                ConsoleKeyInfo key = GetInput();
                if (player.HandleShipPlacementInput(key, length))
                    break;
            }
        }
    }
}
