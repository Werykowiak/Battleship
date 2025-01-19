using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    public class Ship: IShipInterface
    {
        private List<IShipInterface> parts = new List<IShipInterface>();
        private bool sunk = false;

        public Ship(List<IShipInterface> parts)
        {
            this.parts = parts;
        }

        public bool Shoot(int x, int y)
        {
            foreach (IShipInterface part in parts)
            {
                if (part.Shoot(x, y))
                {
                    if (IsHit())
                    {
                        sunk = true;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsHit()
        {
            foreach (ShipPart part in parts)
            {
                if (!part.IsHit())
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

        public List<IShipInterface> getParts()
        {
            return parts;
        }

        public IShipInterface Clone()
        {
            List<IShipInterface> newParts = new List<IShipInterface>();
            foreach (ShipPart part in parts)
            {
                newParts.Add(part.Clone());
            }
            return new Ship(newParts);
        }
    }
}
