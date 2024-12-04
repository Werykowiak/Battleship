﻿using System;
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
        }

        private void buildFleet()
        {
            Player currentPlayer = Player.Player1;

            for (int i = 0; i < 1; ++i) // 1 zamienić na 2
            {
                do
                {
                    Console.Clear();
                    _view.DisplayMap(_model.getShipFleet(currentPlayer).getParts(), _model.getShots(currentPlayer));
                    _view.DisplayRemainingShipsToPlace(_model.getShipFleet(currentPlayer));
                    _view.DisplayBuildInstructions();
                } while (_model.addShipToFleet(currentPlayer));

                currentPlayer = Player.Player2;
            }
        }

        public void Run()
        {
            buildFleet();
            bool gameover = false;


            // Przykładowa rejestracja obserwatorów - jeżeli GameModel zgłosi zdarzenie gameover
            // za pomocą this.notify("GAME_OVER"), to gameover zmieni się na true
            // W ten sposób GameController jest informowany o zmianie stanu gry
            // i może podejmować odpowiednie działania. Ta sama zasada dotyczy się widoku.
            // Reakcją na zdarzenie jest wywołanie funkcji lambda, która zwraca false jeżeli
            // ma się powtórzyć, lub true jeśli zdarzenie po jednym wychwyceniu ma zostać usunięte.
            _model.Connect("GAME_OVER", new Observator.Listener(() => {
                    gameover = true;
                    Console.WriteLine("[GameController] GameModel has informed about game over.");
                return false;
            }));



            Player currentPlayer = Player.Player1;
            while (!gameover)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(currentPlayer).getParts(), _model.getShots(currentPlayer));
                _view.DisplayShotInstructions();
                _model.addShot(currentPlayer);
            }
            // _view.DisplayInstructions();

            //while (!_model.GameOver)
            //{
            //    _view.DisplayBoard(_model.Board);
            //    _view.DisplayShotResult(_model.LastShotResult);
            //    _model.TakeTurn();
            //}

            //_view.DisplayBoard(_model.Board);
            //_view.DisplayGameOverMessage();
        }
    }
}
