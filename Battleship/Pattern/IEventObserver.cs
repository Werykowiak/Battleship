using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Pattern
{
    public interface IEventObserver
    {
        public void Disconnect(int id);
        public void Notify();
        public void NotifyAll();
        public void Notify(string ev);
    }
}
