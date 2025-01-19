﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Model;
using Battleship.View;
using Battleship.Pattern;
using Spectre.Console;

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
            string option;
            do
            {
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
                        var aiDifficulty = SelectAIDifficulty("AI Player");
                        _model.player2 = new AIPlayer("Drugi (AI)", aiDifficulty);
                        BuildFleets();
                        PlayerVsComputerLoop();
                        break;
                    case "Simulation":
                        var ai1Difficulty = SelectAIDifficulty("First AI Player");
                        var ai2Difficulty = SelectAIDifficulty("Second AI Player");
                        _model.player1 = new AIPlayer("Pierwszy (AI)", ai1Difficulty);
                        _model.player2 = new AIPlayer("Drugi (AI)", ai2Difficulty);
                        BuildFleets();
                        ComputerVsComputerLoop();
                        break;
                    case "Options":
                        _view.DisplayOptions();
                        break;
                    case "Achievements":
                        break;
                    case "Ranking":
                        _view.DisplayRanking();
                        break;
                    case "History":
                        _view.DisplayHistory();
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            } while (true);
        }

        public void PlayerVsPlayerLoop()
        {
            _model.resetGame();

            while (!_model.GameOver)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotInstructions();

                int result = _model.addShot(_model.CurrentPlayer);
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotMessage(result);

                if (result == 4)
                {
                    _model.nextTurn();
                }
                Console.ReadKey(true);
            }
            EndGame();
        }

        public void PlayerVsComputerLoop()
        {
            _model.resetGame();

            while (!_model.GameOver)
            {
                // Player's turn
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotInstructions();

                int result = _model.addShot(_model.CurrentPlayer);
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotMessage(result);

                if (result == 4)
                {
                    _model.nextTurn();

                    while (!_model.GameOver)
                    {
                        result = _model.addShot(_model.CurrentPlayer);
                        Console.Clear();
                        _view.DisplayShotMessage(result);
                        
                        if (result == 4)
                        {
                            _model.nextTurn();
                            break;
                        }
                    }
                }
                Console.ReadKey(true);
            }
            EndGame();
        }

        public void ComputerVsComputerLoop()
        {
            _model.resetGame();

            while (!_model.GameOver)
            {
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotInstructions();

                int result = _model.addShot(_model.CurrentPlayer);
                Console.Clear();
                _view.DisplayMap(_model.getShipFleet(_model.CurrentPlayer).getParts(), _model.getShots(_model.CurrentPlayer), _model.CurrentPlayer);
                _view.DisplayShotMessage(result);

                if (result == 4)
                {
                    _model.nextTurn();
                }
                Console.ReadKey(true);
            }
            EndGame();
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

        private AIDifficulty SelectAIDifficulty(string playerName)
        {
            var difficulty = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Select [green]difficulty[/] for {playerName}")
                    .PageSize(3)
                    .AddChoices(new[] {
                        "Easy", "Medium", "Hard"
                    }));
            
            return difficulty switch
            {
                "Easy" => AIDifficulty.Easy,
                "Hard" => AIDifficulty.Hard,
                _ => AIDifficulty.Medium
            };
        }

        private void EndGame()
        {
            Console.Clear();
            _view.DisplayGameSummary(_model.player1, _model.player2, _model.Winner);
            Console.ReadKey(true);
            Console.Clear();
            _model.resetGame();
        }
    }
}
