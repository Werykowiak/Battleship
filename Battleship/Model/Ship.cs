using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    internal class Ship
    {
        private List<ShipPart> parts = new List<ShipPart>();
        private bool sunk = false;

        public Ship(List<ShipPart> parts)
        {
            this.parts = parts;
        }

        public bool Shoot(int x, int y)
        {
            foreach (ShipPart part in parts)
            {
                if (part.Shoot(x, y))
                {
                    if (IsSunk())
                    {
                        sunk = true;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsSunk()
        {
            foreach (ShipPart part in parts)
            {
                if (!part.isHit())
                {
                    return false;
                }
            }
            return true;
        }

        public int getShipLength()
        {
            return parts.Count;
        }

        public List<ShipPart> getParts()
        {
            return parts;
        }

        public Ship clone()
        {
            List<ShipPart> newParts = new List<ShipPart>();
            foreach (ShipPart part in parts)
            {
                newParts.Add(part.clone());
            }
            return new Ship(newParts);
        }
    }
}
