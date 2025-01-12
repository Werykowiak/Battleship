using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Pattern;

namespace Battleship.Model
{
    public class GameModel : EventObserver
    {
        private Player player1 = new Player("Pierwszy");
        private Player player2 = new Player("Drugi");
        public Player CurrentPlayer { get; private set; }
        public bool GameOver { get; private set; }

        public GameModel()
        {
            GameOver = false;
            CurrentPlayer = player1;

            player1.Connect("UPDATE_FLEET_VIEW", new Observator.Listener(() => {
                this.Notify("UPDATE_FLEET_VIEW");
                return false;
            }));
            player1.Connect("UPDATE_BUILDER_VIEW", new Observator.Listener(() => {
                this.Notify("UPDATE_BUILDER_VIEW");
                return false;
            }));
            player1.Connect("DISPLAY_BUILD_INSTRUCTIONS", new Observator.Listener(() =>
            {
                this.Notify("DISPLAY_BUILD_INSTRUCTIONS");
                return false;
            }));

            player2.Connect("UPDATE_FLEET_VIEW", new Observator.Listener(() => {
                this.Notify("UPDATE_FLEET_VIEW");
                return false;
            }));
            player2.Connect("UPDATE_BUILDER_VIEW", new Observator.Listener(() => {
                this.Notify("UPDATE_BUILDER_VIEW");
                return false;
            }));
            player2.Connect("DISPLAY_BUILD_INSTRUCTIONS", new Observator.Listener(() => {
                this.Notify("DISPLAY_BUILD_INSTRUCTIONS");
                return false;
            }));
        }

        public void BuildFleets()
        {
            CurrentPlayer = player1;
            player1.BuildFleet();
            CurrentPlayer = player2;
            player2.BuildFleet();
            CurrentPlayer = player1;
        }

        public ShipFleet getShipFleet(Player player)
        {
            return player == player1 ? player1.GetFleet() : player2.GetFleet();
        }

        public List<Shot> getShots(Player player)
        {
            return player == player1 ? player1.GetShots() : player2.GetShots();
        }

        public Ship? getCurrentPlaceholderShip()
        {
            return CurrentPlayer.CurrentPlaceholderShip;
        }

        public void addShot(Player player)
        {
            string? coordinates = Console.ReadLine();
            if (string.IsNullOrEmpty(coordinates))
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            Vector2i pos = new Vector2i(coordinates[0] - 'A', coordinates[1] - '1');
            
            if (player == player1)
                player1.AddShot(pos);
            else
                player2.AddShot(pos);
        }

        public void nextTurn()
        {
            CurrentPlayer = CurrentPlayer == player1 ? player2 : player1;
        }
    }
}
