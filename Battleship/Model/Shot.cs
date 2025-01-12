using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    public class Shot
    {
        private Vector2i position;

        public Shot(Vector2i pos)
        {
            position = pos;
        }

        public Vector2i getPosition() => position;

        public void display()
        {
            Console.Write("!");
        }
    }
}
