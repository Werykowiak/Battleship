using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    internal class ShipFleet
    {
        private List<Ship> ships;

        public ShipFleet()
        {
            this.ships = new List<Ship>();
        }

        public void addShip(Ship ship)
        {
            ships.Add(ship);
        }

        public bool Shoot(int x, int y)
        {
            foreach (Ship ship in ships)
            {
                if (ship.Shoot(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSunk()
        {
            foreach (Ship ship in ships)
            {
                if (!ship.IsSunk())
                {
                    return false;
                }
            }
            return true;
        }

        public int getShipCount(int length)
        {
            int count = 0;
            foreach (Ship ship in ships)
            {
                if (ship.getShipLength() == length)
                {
                    count++;
                }
            }
            return count;
        }

        public List<ShipPart> getParts()
        {
            List<ShipPart> parts = new List<ShipPart>();
            foreach (Ship ship in ships)
            {
                parts.AddRange(ship.getParts());
            }
            return parts;
        }
    }
}
