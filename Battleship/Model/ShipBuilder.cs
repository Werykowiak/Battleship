using Battleship.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    internal class ShipBuilder
    {
        private int MAX_5_SHIPS = 1;
        private int MAX_4_SHIPS = 2;
        private int MAX_3_SHIPS = 3;

        private List<ShipPart> parts = new List<ShipPart>();
        private AssetManager assetManager = AssetManager.Instance;

        private bool ValidateCount(int length, ShipFleet fleet)
        {
            if (length == 5)
                return fleet.getShipCount(5) < MAX_5_SHIPS;
            else if (length == 4)
                return fleet.getShipCount(4) < MAX_4_SHIPS;
            else if (length == 3)
                return fleet.getShipCount(3) < MAX_3_SHIPS;

            return false;
        }

        private bool ValidateOverlap(Vector2i pos1, Vector2i pos2, ShipFleet fleet)
        {
            foreach (ShipPart part in fleet.getParts())
                if (part.getPosition().x >= pos1.x && part.getPosition().x <= pos2.x && part.getPosition().y >= pos1.y && part.getPosition().y <= pos2.y)
                    return false; // Overlap

            return true; // No overlap
        }

        public bool AddShip(Vector2i pos1, Vector2i pos2, ShipFleet fleet)
        {
            if (pos1.x == pos2.x) // Vertical
            {
                if (pos1.y > pos2.y)
                {
                    Vector2i temp = pos1;
                    pos1 = pos2;
                    pos2 = temp;
                }

                if (ValidateCount(pos2.y - pos1.y + 1, fleet) == false)
                    return false;

                if (ValidateOverlap(pos1, pos2, fleet) == false)
                    return false;

                // Tutaj zakładamy że akurat wczytujemy statki gracza, więc custom = true
                string skin = AssetManager.Instance.GetSkin((ShipType)(pos2.y - pos1.y + 1), true);
                for (int i = pos1.y, c = 0; i <= pos2.y; i++, c++)
                    parts.Add(new ShipPart(pos1.x, i, skin[c]));
            }
            else if (pos1.y == pos2.y) // Horizontal
            {
                if (pos1.x > pos2.x)
                {
                    Vector2i temp = pos1;
                    pos1 = pos2;
                    pos2 = temp;
                }

                if (ValidateCount(pos2.x - pos1.x + 1, fleet) == false)
                    return false;

                if (ValidateOverlap(pos1, pos2, fleet) == false)
                    return false;

                // Tutaj zakładamy że akurat wczytujemy statki gracza, więc custom = true
                string skin = AssetManager.Instance.GetSkin((ShipType)(pos2.x - pos1.x + 1), true);
                for (int i = pos1.x, c = 0; i <= pos2.x; i++, c++)
                    parts.Add(new ShipPart(i, pos1.y, skin[c]));
            }
            else
                return false;

            return true;
        }

        public Ship Build()
        {
            Ship ship = new Ship(new List<ShipPart>(parts));
            parts.Clear();
            return ship;
        }
    }
}
