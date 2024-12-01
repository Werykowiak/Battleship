using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Model;

namespace Battleship.View
{
    internal class GameView
    {
        public void DisplayBuildInstructions()
        {
            Console.WriteLine("Enter the coordinates of the ship: ");
        }

        public void DisplayRemainingShipsToPlace(ShipFleet fleet)
        {
            Console.WriteLine("Remaining ships to place: ");
            Console.WriteLine("Remaining 5-length ships: " + (1 - fleet.getShipCount(5)).ToString());
            Console.WriteLine("Remaining 4-length ships: " + (2 - fleet.getShipCount(4)).ToString());
            Console.WriteLine("Remaining 3-length ships: " + (3 - fleet.getShipCount(3)).ToString());
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

        public void DisplayShotInstructions()
        {
            Console.WriteLine("Enter the coordinates of the shot: ");
        }
    }
}
