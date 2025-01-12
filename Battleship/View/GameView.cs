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
            Console.WriteLine("      A    B    C    D    E    F    G    H    I    J   ");

            for (int i = 1; i <= 10; i++)
            {
                Console.Write($"{i,2}    ");
                for (int j = 0; j < 10; j++)
                    Console.Write("     ");

                Console.WriteLine();
                Console.WriteLine();
            }

            int endTop = Console.GetCursorPosition().Top;
            int endLeft = Console.GetCursorPosition().Left;

            foreach (ShipPart part in shipParts)
            {
                Console.SetCursorPosition(part.getPosition().x * 5 + 6, part.getPosition().y * 2 + 1);
                part.display();
            }

            foreach (Shot shot in firedShots)
            {
                Console.SetCursorPosition(shot.getPosition().x * 5 + 6, shot.getPosition().y * 2 + 1);
                shot.display();
            }

            Console.SetCursorPosition(endLeft, endTop);
        }

        public void DisplayBuilderMap(List<ShipPart> shipParts, Ship? placeholderShip)
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

            int endTop = Console.GetCursorPosition().Top;
            int endLeft = Console.GetCursorPosition().Left;

            // rozstawienie juz postawionych statkow
            foreach (ShipPart part in shipParts)
            {
                Console.SetCursorPosition(part.getPosition().x * 5 + 6, part.getPosition().y * 2 + 1);
                part.display();
            }

            // rozstawienie placeholdera uzytkownika
            if (placeholderShip != null)
            {
                foreach (ShipPart part in placeholderShip.getParts())
                {
                    Console.SetCursorPosition(part.getPosition().x * 5 + 6, part.getPosition().y * 2 + 1);
                    part.display();
                }
            }

            Console.SetCursorPosition(endLeft, endTop);
        }

        public void DisplayShotInstructions()
        {
            Console.WriteLine("Enter the coordinates of the shot: ");
        }

        public ConsoleKey GetInput()
        {
            return Console.ReadKey(true).Key;
        }

        public void DisplayCurrentPlayer(string playerName)
        {
            (int left, int top) = Console.GetCursorPosition();
            
            Console.SetCursorPosition(Console.WindowWidth - playerName.Length - 15, 0);
            Console.Write("Tura gracza: " + playerName);
            
            Console.SetCursorPosition(left, top);
        }
    }
}
