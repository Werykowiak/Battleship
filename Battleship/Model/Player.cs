using Battleship.Pattern;

namespace Battleship.Model
{
    public class Player : EventObserver
    {
        public string name;
        private ShipFleet fleet = new ShipFleet();
        private List<Shot> shots = new List<Shot>();
        private ShipBuilder shipBuilder = new ShipBuilder();
        private Vector2i position = new Vector2i(0, 0);
        private Orientation orientation = Orientation.Horizontal;
        private const int BOARD_SIZE = 10;

        public Ship? CurrentPlaceholderShip { get; private set; }

        public Player(string name)
        {
            this.name = name;
        }

        public bool CanAddShipOfLength(int length)
        {
            int currentCount = fleet.getShipCount(length);
            int maxCount = length == 5 ? 1 : length == 4 ? 2 : 3;
            return currentCount < maxCount;
        }

        public void UpdatePlaceholderShip(int length)
        {
            CurrentPlaceholderShip = shipBuilder
                .SetLength(length)
                .SetOrientation(orientation)
                .SetStartPosition(position)
                .Build();
        }

        public bool HandleShipPlacementInput(ConsoleKeyInfo key, int length)
        {
            switch (key.Key)
            {
                case ConsoleKey.W when position.y > 0:
                    position.y--; break;
                case ConsoleKey.S when CanMoveDown(length):
                    position.y++; break;
                case ConsoleKey.A when position.x > 0:
                    position.x--; break;
                case ConsoleKey.D when CanMoveRight(length):
                    position.x++; break;
                case ConsoleKey.R:
                    orientation = orientation == Orientation.Horizontal ? 
                        Orientation.Vertical : Orientation.Horizontal;
                    break;
                case ConsoleKey.Enter:
                    return TryPlaceShip(length);
            }
            return false;
        }

        private bool CanMoveDown(int length) =>
            orientation == Orientation.Vertical ? 
                position.y < BOARD_SIZE - length : 
                position.y < BOARD_SIZE - 1;

        private bool CanMoveRight(int length) =>
            orientation == Orientation.Horizontal ? 
                position.x < BOARD_SIZE - length : 
                position.x < BOARD_SIZE - 1;

        private bool TryPlaceShip(int length)
        {
            Ship ship = shipBuilder
                .SetLength(length)
                .SetOrientation(orientation)
                .SetStartPosition(position)
                .Build();

            if (!IsValidPlacement(ship))
                return false;

            fleet.addShip(ship);
            position = new Vector2i(0, 0);
            return true;
        }

        private bool IsValidPlacement(Ship ship)
        {
            Vector2i startPos = ship.getParts()[0].getPosition();
            int length = ship.getParts().Count;

            if (!IsWithinBounds(startPos, length, orientation))
                return false;

            foreach (ShipPart newPart in ship.getParts())
            {
                foreach (Ship existingShip in fleet.GetShips())
                {
                    foreach (ShipPart existingPart in existingShip.getParts())
                    {
                        if (newPart.getPosition().x == existingPart.getPosition().x &&
                            newPart.getPosition().y == existingPart.getPosition().y)
                            return false;
                    }
                }
            }
            return true;
        }

        private bool IsWithinBounds(Vector2i position, int length, Orientation orientation)
        {
            if (position.x < 0 || position.y < 0)
                return false;

            if (orientation == Orientation.Horizontal)
            {
                if (position.x + length > BOARD_SIZE)
                    return false;
                if (position.y >= BOARD_SIZE)
                    return false;
            }
            else
            {
                if (position.y + length > BOARD_SIZE)
                    return false;
                if (position.x >= BOARD_SIZE)
                    return false;
            }

            return true;
        }

        public void AddShot(Vector2i pos)
        {
            shots.Add(new Shot(pos));
        }

        public ShipFleet GetFleet() => fleet;
        public List<Shot> GetShots() => shots;
    }
} 