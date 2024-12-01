using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    internal class Shot
    {
        private Vector2i position { get; set; }

        public Shot(Vector2i position)
        {
            this.position = position;
        }

        public Vector2i getPosition()
        {
            return position;
        }

        public void display()
        {
            Console.Write("!");
        }
    }
}
