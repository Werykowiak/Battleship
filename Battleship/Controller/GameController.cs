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

            _model.Connect("UPDATE_FLEET_VIEW", new Observator.Listener(() => {
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer));
                _view.DisplayRemainingShipsToPlace(_model.getShipFleet(_model.CurrentPlayer));
                _view.DisplayCurrentPlayer(_model.CurrentPlayer.name);
                return false;
            }));

            _model.Connect("DISPLAY_BUILD_INSTRUCTIONS", new Observator.Listener(() => {
                _view.DisplayBuildInstructions(1);
                return false;
            }));

            _model.Connect("UPDATE_BUILDER_VIEW", new Observator.Listener(() => {
                _view.DisplayBuilderMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getCurrentPlaceholderShip());
                _view.DisplayCurrentPlayer(_model.CurrentPlayer.name);
                return false;
            }));

            _model.Connect("DISPLAY_SHOT_INSTRUCTIONS", new Observator.Listener(() => {
                _view.DisplayShotInstructions();
                return false;
            }));
        }

        public void Run()
        {
            _model.BuildFleets();
            
            while (!_model.GameOver)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer));
                _view.DisplayShotInstructions();
                _model.addShot(_model.CurrentPlayer);
                _model.nextTurn();
            }
        }
    }
}
