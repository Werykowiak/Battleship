using Spectre.Console;
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
        private char representation = '!';

        public Shot(Vector2i pos, char representation)
        {
            position = pos;
            this.representation = representation;
        }

        public Vector2i getPosition() => position;

        public void display()
        {
            if(representation == '!')
                AnsiConsole.Write(new Markup($"[red]{representation}[/]"));
            else
                AnsiConsole.Write(new Markup($"[yellow]{representation}[/]"));
        }
    }
}
