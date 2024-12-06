using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    internal class ShipPart
    {
        private Vector2i position { get; set; }
        private bool hit { get; set; }
        private char representation { get; set; }

        public ShipPart(int x, int y, char representation = '%')
        {
            position = new Vector2i(x, y);
            this.representation = representation;
            hit = false;
        }

        public bool Shoot(int x, int y)
        {
            if (position.x == x && position.y == y)
            {
                hit = true;
                return true;
            }
            return false;
        }

        public bool isHit()
        {
            return hit;
        }

        public Vector2i getPosition()
        {
            return position;
        }

        public void setPosition(Vector2i position)
        {
            this.position = position;
        }

        public void display()
        {
            if (hit)
                Console.Write("X");
            else
                Console.Write(representation);
        }
        public ShipPart clone()
        {
            return new ShipPart(position.x, position.y);
        }
    }
}
