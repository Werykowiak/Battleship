using System;

namespace Battleship.Pattern
{
   public class EventObserver : Observator, IEventObserver
    {
        public int Connect(string evt, Observator.Listener observer)
        {
            return RegisterObserver(evt, observer);
        }

        public int Connect(Observator.Listener observer)
        {
            return RegisterObserver("", observer);
        }

        public void Disconnect(int id)
        {
            UnregisterObserver(id);
        }

        public void NotifyAll()
        {
            NotifyAllObservers();
        }

        public void Notify(string evt) 
        {
            NotifyObserver(evt);
        }

        public void Notify()
        {
            NotifyObserver("");
        }
   }
}
