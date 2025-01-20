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
        protected bool isCustom = false;

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

    public enum AIDifficulty
    {
        Easy,
        Medium,
        Hard
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
                .SetCustomSkin(isCustom)
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
                 case ConsoleKey.C:
                    isCustom = !isCustom;
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
            Ship ship = (Ship)CurrentPlaceholderShip.Clone();

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
        protected Random random = new Random();
        protected AIDifficulty difficulty;
        protected List<Vector2i> successfulHits = new List<Vector2i>();

        public AIPlayer(string name, AIDifficulty difficulty = AIDifficulty.Medium) : base(name)
        {
            this.difficulty = difficulty;
        }

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
                        .SetCustomSkin(false)
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
            Vector2i pos = difficulty switch
            {
                AIDifficulty.Easy => GetEasyShot(),
                AIDifficulty.Medium => GetMediumShot(),
                AIDifficulty.Hard => GetHardShot(targetFleet),
                _ => GetMediumShot()
            };

            foreach (ShipPart part in targetFleet.getParts())
            {
                if (part.Shoot(pos.x, pos.y))
                {
                    shots.Add(new Shot(pos, '!'));
                    if (difficulty != AIDifficulty.Easy)
                    {
                        successfulHits.Add(pos);
                    }
                    return 0;
                }
            }

            shots.Add(new Shot(pos, 'O'));
            return 4;
        }

        private Vector2i GetEasyShot()
        {
            Vector2i pos;
            do
            {
                pos = new Vector2i(random.Next(BOARD_SIZE), random.Next(BOARD_SIZE));
            } while (!IsValidShot(pos));
            return pos;
        }

        private Vector2i GetMediumShot()
        {
            if (successfulHits.Count > 0 && random.Next(100) < 70)
            {
                Vector2i lastHit = successfulHits.Last();
                Vector2i[] adjacentPositions = new[]
                {
                    new Vector2i(lastHit.x - 1, lastHit.y),
                    new Vector2i(lastHit.x + 1, lastHit.y),
                    new Vector2i(lastHit.x, lastHit.y - 1),
                    new Vector2i(lastHit.x, lastHit.y + 1)
                };

                var validPositions = adjacentPositions.Where(pos => 
                    pos.x >= 0 && pos.x < BOARD_SIZE && 
                    pos.y >= 0 && pos.y < BOARD_SIZE && 
                    IsValidShot(pos)).ToList();

                if (validPositions.Any())
                {
                    return validPositions[random.Next(validPositions.Count)];
                }
            }

            return GetEasyShot();
        }

        private Vector2i GetHardShot(ShipFleet targetFleet)
        {
            if (successfulHits.Count > 0)
            {
                Vector2i lastHit = successfulHits.Last();
                bool horizontal = successfulHits.Count > 1 && 
                    successfulHits.Any(h => h.x != lastHit.x && h.y == lastHit.y);

                List<Vector2i> potentialShots = new List<Vector2i>();
                if (horizontal)
                {
                    potentialShots.Add(new Vector2i(lastHit.x - 1, lastHit.y));
                    potentialShots.Add(new Vector2i(lastHit.x + 1, lastHit.y));
                }
                else
                {
                    potentialShots.Add(new Vector2i(lastHit.x, lastHit.y - 1));
                    potentialShots.Add(new Vector2i(lastHit.x, lastHit.y + 1));
                }

                var validShots = potentialShots.Where(pos => 
                    pos.x >= 0 && pos.x < BOARD_SIZE && 
                    pos.y >= 0 && pos.y < BOARD_SIZE && 
                    IsValidShot(pos)).ToList();

                if (validShots.Any())
                {
                    return validShots[random.Next(validShots.Count)];
                }
            }

            int attempts = 0;
            const int MAX_ATTEMPTS = 100;
            Vector2i pos;

            do
            {
                pos = new Vector2i(random.Next(BOARD_SIZE), random.Next(BOARD_SIZE));
                attempts++;
                
                if (attempts > MAX_ATTEMPTS)
                {
                    for (int x = 0; x < BOARD_SIZE; x++)
                    {
                        for (int y = 0; y < BOARD_SIZE; y++)
                        {
                            pos = new Vector2i(x, y);
                            if (IsValidShot(pos))
                            {
                                return pos;
                            }
                        }
                    }
                }
            } while (!IsValidShot(pos) || (pos.x + pos.y) % 2 != 0);

            return pos;
        }

        private bool IsValidShot(Vector2i pos)
        {
            return !shots.Any(shot => 
                shot.getPosition().x == pos.x && 
                shot.getPosition().y == pos.y
            );
        }
    }
} 