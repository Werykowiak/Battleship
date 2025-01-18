using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Model;
using Battleship.View;
using Battleship.Pattern;

namespace Battleship.Controller
{
    internal class GameController
    {
        private GameModel _model;
        private GameView _view;

        public GameController(GameModel model, GameView view)
        {
            _model = model;
            _view = view;
            
            // Subscribe to model events
            _model.Connect("GAME_OVER", new Observator.Listener(() => {
                Console.WriteLine("[GameController] GameModel has informed about game over.");
                return false;
            }));
        }

        public void Run()
        {

            // Rozstawianie floty
            string option;
            do{
                option = _view.MainMenu();
                switch (option)
                {
                    case "Player vs Player":
                        _model.player1 = new Player("Pierwszy");
                        _model.player2 = new Player("Drugi");
                        BuildFleets();
                        PlayerVsPlayerLoop();
                        break;
                    case "Player vs Computer":
                        _model.player1 = new Player("Pierwszy");
                        _model.player2 = new AIPlayer("Drugi");
                        BuildFleets();
                        PlayerVsComputerLoop();
                        break;
                    case "Simulation":
                        _model.player1 = new AIPlayer("Pierwszy");
                        _model.player2 = new AIPlayer("Drugi");
                        BuildFleets();
                        ComputerVsComputerLoop();
                        break;
                    case "Options":
                        _view.DisplayOptions();
                        break;
                    case "Achievements":
                        break;
                    case "Ranking":
                        break;
                    case "History":
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            } while (true);
        }

        public void PlayerVsPlayerLoop()
        {
            while (!_model.GameOver)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotInstructions();

                int result = _model.addShot(_model.CurrentPlayer);
                _view.DisplayShotMessage(result);

                if (result == 4)
                {
                    _model.nextTurn();
                }
                Console.ReadKey(true);
            }
            Console.Clear();
        }

        public void PlayerVsComputerLoop()
        {
            while (!_model.GameOver)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotInstructions();

                int result = _model.addShot(_model.CurrentPlayer);
                _view.DisplayShotMessage(result);

                if (result == 4)
                {
                    _model.nextTurn();

                    while (!_model.GameOver)
                    {
                        result =_model.addShot(_model.CurrentPlayer);
                        if (result == 4)
                        {
                            _model.nextTurn();
                            break;
                        }
                    }
                }
                Console.ReadKey(true);
            }
            Console.Clear();
        }

        public void ComputerVsComputerLoop()
        {
            while (!_model.GameOver)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotInstructions();

                int result = _model.addShot(_model.CurrentPlayer);
                _view.DisplayShotMessage(result);

                if (result == 4)
                {
                    _model.nextTurn();
                }
                Console.ReadKey(true);
            }
            Console.Clear();
        }
        private void BuildFleets()
        {
            Console.Clear();
            _view.BuildFleetForPlayer(_model.player1);
            _model.nextTurn();
            Console.Clear();
            _view.BuildFleetForPlayer(_model.player2);
            _model.nextTurn();
        }
    }
}
