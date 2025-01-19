using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    public class ShipPart : IShipInterface
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

        public bool IsHit()
        {
            return hit;
        }

        public Vector2i GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector2i position)
        {
            this.position = position;
        }

        public void Display()
        {
            if (hit)
                AnsiConsole.Write(new Markup("[red]X[/]"));
            else
                Console.Write(representation);
        }
        public IShipInterface Clone()
        {
            return new ShipPart(position.x, position.y);
        }
    }
}
