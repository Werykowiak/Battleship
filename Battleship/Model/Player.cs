using Battleship.Pattern;
using System.ComponentModel.Design;

namespace Battleship.Model
{
    public interface IPlayer : IEventObserver
    {
        string name { get; }
        Ship? CurrentPlaceholderShip { get; }
        ShipFleet GetFleet();
        List<Shot> GetShots();
        void AddShot(Vector2i pos, char rep);
        void BuildFleet(Action<List<IShipInterface>, Ship?> displayCallback);
        bool CanAddShipOfLength(int length);
        int AddShot(ShipFleet targetFleet);
    }

    public abstract class BasePlayer : EventObserver, IPlayer
    {
        public string name { get; protected set; }
        public Ship? CurrentPlaceholderShip { get; protected set; }
        protected ShipFleet fleet = new ShipFleet();
        protected List<Shot> shots = new List<Shot>();
        protected ShipBuilder shipBuilder = new ShipBuilder();
        protected Vector2i position = new Vector2i(0, 0);
        protected Orientation orientation = Orientation.Horizontal;
        protected const int BOARD_SIZE = 10;

        public BasePlayer(string name)
        {
            this.name = name;
        }

        public ShipFleet GetFleet() => fleet;
        public List<Shot> GetShots() => shots;
        public void AddShot(Vector2i pos, char rep) => shots.Add(new Shot(pos, rep));

        public bool CanAddShipOfLength(int length)
        {
            int currentCount = fleet.getShipCount(length);
            int maxCount = length == 5 ? 1 : length == 4 ? 2 : 3;
            return currentCount < maxCount;
        }

        protected bool IsValidPlacement(Ship ship)
        {
            Vector2i startPos = ((ShipPart)ship.getParts()[0]).GetPosition();
            int length = ship.getParts().Count;

            if (!IsWithinBounds(startPos, length, orientation))
                return false;

            foreach (ShipPart newPart in ship.getParts())
            {
                foreach (Ship existingShip in fleet.GetShips())
                {
                    foreach (ShipPart existingPart in existingShip.getParts())
                    {
                        if (newPart.GetPosition().x == existingPart.GetPosition().x &&
                            newPart.GetPosition().y == existingPart.GetPosition().y)
                            return false;
                    }
                }
            }
            return true;
        }

        protected bool IsWithinBounds(Vector2i position, int length, Orientation orientation)
        {
            if (position.x < 0 || position.y < 0)
                return false;

            if (orientation == Orientation.Horizontal)
                return position.x + length <= BOARD_SIZE && position.y < BOARD_SIZE;
            else
                return position.y + length <= BOARD_SIZE && position.x < BOARD_SIZE;
        }

        public abstract void BuildFleet(Action<List<IShipInterface>, Ship?> displayCallback);
        public abstract int AddShot(ShipFleet targetFleet);
    }

    public class Player : BasePlayer
    {
        public Player(string name) : base(name) { }

        public override void BuildFleet(Action<List<IShipInterface>, Ship?> displayCallback)
        {
            while (!fleet.isComplete())
            {
                displayCallback(fleet.getParts(), CurrentPlaceholderShip);
                ConsoleKeyInfo input = Console.ReadKey(true);
                HandleFleetBuilding(input);
            }
        }

        private bool HandleFleetBuilding(ConsoleKeyInfo input)
        {
            if (CurrentPlaceholderShip == null)
            {
                if (!int.TryParse(input.KeyChar.ToString(), out int shipType) || shipType < 1 || shipType > 3)
                    return false;

                int length = shipType == 1 ? 5 : shipType == 2 ? 4 : 3;
                
                if (!CanAddShipOfLength(length))
                    return false;

                if (input.Key != ConsoleKey.Enter)
                {
                    UpdatePlaceholderShip(length);
                    return false;
                }
            }
            
            int currentLength = CurrentPlaceholderShip?.getParts().Count ?? 0;
            if (HandleShipPlacementInput(input, currentLength))
            {
                CurrentPlaceholderShip = null;
                return false;
            }

            return false;
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
            bool moved = false;
            
            switch (key.Key)
            {
                case ConsoleKey.W when position.y > 0:
                    position.y--;
                    moved = true;
                    break;
                case ConsoleKey.S when CanMoveDown(length):
                    position.y++;
                    moved = true;
                    break;
                case ConsoleKey.A when position.x > 0:
                    position.x--;
                    moved = true;
                    break;
                case ConsoleKey.D when CanMoveRight(length):
                    position.x++;
                    moved = true;
                    break;
                case ConsoleKey.R:
                    orientation = orientation == Orientation.Horizontal ? 
                        Orientation.Vertical : Orientation.Horizontal;
                    moved = true;
                    break;
                case ConsoleKey.Enter:
                    return TryPlaceShip(length);
            }

            if (moved && CurrentPlaceholderShip != null)
            {
                UpdatePlaceholderShip(length);
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

        public override int AddShot(ShipFleet targetFleet)
        {
            string? coordinates = Console.ReadLine();
            if (string.IsNullOrEmpty(coordinates) || coordinates.Length < 2)
                return 1;

            if (!char.IsLetter(coordinates[0]) || coordinates[0] < 'A' || coordinates[0] > 'J')
                return 2;

            string numberPart = coordinates.Substring(1);
            if (!int.TryParse(numberPart, out int number) || number < 1 || number > 10)
                return 3;

            Vector2i pos = new Vector2i(coordinates[0] - 'A', number - 1);
            
            if (shots.Any(shot => shot.getPosition().x == pos.x && shot.getPosition().y == pos.y))
                return 1;

            foreach (ShipPart part in targetFleet.getParts())
            {
                if (part.Shoot(pos.x, pos.y)){
                    shots.Add(new Shot(pos, '!'));
                    return 0;
                }
            }

            shots.Add(new Shot(pos, 'O'));
            return 4;
        }
    }

    public class AIPlayer : BasePlayer
    {
        private Random random = new Random();

        public AIPlayer(string name) : base(name) { }

        public override void BuildFleet(Action<List<IShipInterface>, Ship?> displayCallback)
        {
            int[] shipLengths = new[] { 5, 4, 4, 3, 3, 3 };
            
            foreach (int length in shipLengths)
            {
                while (true)
                {
                    position = new Vector2i(random.Next(BOARD_SIZE), random.Next(BOARD_SIZE));
                    orientation = random.Next(2) == 0 ? Orientation.Horizontal : Orientation.Vertical;

                    Ship ship = shipBuilder
                        .SetLength(length)
                        .SetOrientation(orientation)
                        .SetStartPosition(position)
                        .Build();

                    if (IsValidPlacement(ship))
                    {
                        fleet.addShip(ship);
                        displayCallback(fleet.getParts(), null);
                        break;
                    }
                }
            }
        }

        public override int AddShot(ShipFleet targetFleet)
        {
            Vector2i pos;
            bool validShot;

            do
            {
                pos = new Vector2i(random.Next(BOARD_SIZE), random.Next(BOARD_SIZE));
                validShot = !shots.Any(shot => 
                    shot.getPosition().x == pos.x && 
                    shot.getPosition().y == pos.y
                );
            } while (!validShot);

            foreach (ShipPart part in targetFleet.getParts())
            {
                if (part.Shoot(pos.x, pos.y))
                {
                    shots.Add(new Shot(pos, '!'));
                    return 0;
                }
            }

            shots.Add(new Shot(pos, 'O'));
            return 4;
        }
    }
} 