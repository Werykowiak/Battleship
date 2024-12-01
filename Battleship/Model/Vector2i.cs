using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    internal class Vector2i
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Vector2i other)
        {
            return x == other.x && y == other.y;
        }
    }
}
