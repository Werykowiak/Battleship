using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Model
{
    public interface IShipInterface
    {
        public bool Shoot(int x, int y);
        public bool IsHit();
        public IShipInterface Clone();
    }
}
