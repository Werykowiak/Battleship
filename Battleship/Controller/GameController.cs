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
            Console.Clear();
            _view.BuildFleetForPlayer(_model.player1);
            _model.nextTurn();
            Console.Clear();
            _view.BuildFleetForPlayer(_model.player2);
            _model.nextTurn(); // dlatego jestesmy znow na 1 graczu

            // Reszta gry
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
        }
    }
}
