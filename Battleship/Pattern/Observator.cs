namespace Battleship.Pattern
{
    internal class Observator
    {
        public delegate bool Listener();

        private Dictionary<string, Dictionary<int, Listener>> _observers;
        private int _curId;

        public Observator()
        {
            _observers = new Dictionary<string, Dictionary<int, Listener>>();
            _curId = 0;
        }

        private int GetNewId()
        {
            return ++_curId;
        }

        private void UnregisterIds(List<int> ids)
        {
            if (ids.Count == 0) return;

            foreach (int id in ids)
            {
                foreach (var listeners in _observers)
                {
                    var eventListeners = listeners.Value;
                    eventListeners.Remove(id);
                }
            }
        }

        public int RegisterObserver(string evt, Listener observer)
        {
            int id = GetNewId();
            if (!_observers.ContainsKey(evt))
            {
                _observers[evt] = new Dictionary<int, Listener>();
            }
            _observers[evt].Add(id, observer);
            return id;
        }

        public void UnregisterObserver(string evt, int id)
        {
            if (_observers.TryGetValue(evt, out var eventListeners))
                eventListeners.Remove(id);
        }

        public void UnregisterObserver(int id)
        {
            foreach (var eventsObservers in _observers)
            {
                int prevSize = eventsObservers.Value.Count;
                UnregisterObserver(eventsObservers.Key, id);

                if (prevSize != eventsObservers.Value.Count)
                    return;
            }
        }

        public void NotifyObserver(string evt)
        {
            List<int> toRemove = new List<int>();

            if (_observers.TryGetValue(evt, out var eventListeners))
                foreach (var obs in eventListeners)
                    if (obs.Value())
                        toRemove.Add(obs.Key);

            UnregisterIds(toRemove);
        }

        public void NotifyAllObservers()
        {
            List<int> toRemove = new List<int>();

            foreach (var obsEvents in _observers)
                foreach (var obs in obsEvents.Value)
                    if (obs.Value())
                        toRemove.Add(obs.Key);

            UnregisterIds(toRemove);
        }
    }
}