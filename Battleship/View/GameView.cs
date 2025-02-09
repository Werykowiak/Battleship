﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Model;
using Battleship.Pattern;
using Spectre.Console;
namespace Battleship.View
{
    internal class GameView : EventObserver
    {
        private const int MAP_WIDTH = 50;
        private const int MAP_SPACING = 15;
        private AssetManager _assetManager = AssetManager.Instance;
        public void DisplayBuildInstructions(int i)
        {
            switch (i)
            {
                case 1:
                    Console.WriteLine("Choose a ship to place: ");
                    break;
                case 2:
                    Console.WriteLine("Choose where to place it: ");
                    break;
            }
        }

        public void DisplayRemainingShipsToPlace(ShipFleet fleet)
        {
            Console.WriteLine("Remaining ships to place: ");
            Console.WriteLine("1. Remaining 5-length ships: " + (1 - fleet.getShipCount(5)).ToString());
            Console.WriteLine("2. Remaining 4-length ships: " + (2 - fleet.getShipCount(4)).ToString());
            Console.WriteLine("3. Remaining 3-length ships: " + (3 - fleet.getShipCount(3)).ToString());
            Console.WriteLine("");
        }

        public void DisplayMap(List<IShipInterface> shipParts, List<Shot> firedShots, IPlayer player)
        {
            DrawBaseGrid(0);
            foreach (Shot shot in firedShots)
            {
                Console.SetCursorPosition(shot.getPosition().x * 5 + 6, shot.getPosition().y * 2 + 2);
                shot.display();
            }

            DrawBaseGrid(MAP_WIDTH + MAP_SPACING);
            DisplayShipParts(shipParts, MAP_WIDTH + MAP_SPACING);

            Console.SetCursorPosition(0, 22);
            DisplayCurrentPlayer(player.name);
        }

        public void DisplayBuilderMap(List<IShipInterface> shipParts, Ship? placeholderShip)
        {
            DrawBaseGrid(0);
            DisplayShipParts(shipParts, 0);

            if (placeholderShip != null)
            {
                DisplayShipParts(placeholderShip.getParts(), 0);
            }
        }

        private void DrawBaseGrid(int leftOffset)
        {
            Console.SetCursorPosition(leftOffset, 0);
            AnsiConsole.Write(new Markup($"[{_assetManager.MapColor}]      A    B    C    D    E    F    G    H    I    J   [/]"));

            for (int i = 1; i <= 10; i++)
            {
                Console.SetCursorPosition(leftOffset, i * 2);
                AnsiConsole.Write(new Markup($"[{_assetManager.MapColor}]{i,2}    [/]"));
                for (int j = 0; j < 10; j++)
                    Console.Write("     ");

                Console.WriteLine();
            }
        }

        private void DisplayShipParts(List<IShipInterface> parts, int leftOffset)
        {
            (int left, int top) = Console.GetCursorPosition();
            foreach (ShipPart part in parts)
            {
                Console.SetCursorPosition(part.GetPosition().x * 5 + 6 + leftOffset, part.GetPosition().y * 2 + 2);
                part.Display();
            }
            Console.SetCursorPosition(left, top);
        }

        public void DisplayShotInstructions()
        {
            Console.WriteLine("Enter the coordinates of the shot: ");
        }

        public ConsoleKeyInfo GetInput()
        {
            return Console.ReadKey(true);
        }

        public void DisplayCurrentPlayer(string playerName)
        {
            (int left, int top) = Console.GetCursorPosition();

            Console.SetCursorPosition(Console.WindowWidth - playerName.Length - 15, 22);
            Console.Write("Tura gracza: " + playerName);

            Console.SetCursorPosition(left, top);
        }

        public void BuildFleetForPlayer(IPlayer player)
        {
            player.BuildFleet((shipParts, placeholderShip) =>
            {
                Console.Clear();
                DisplayBuilderMap(shipParts, placeholderShip);
                DisplayCurrentPlayer(player.name);

                if (player is Player && placeholderShip == null)
                {
                    DisplayRemainingShipsToPlace(player.GetFleet());
                    DisplayBuildInstructions(1);
                }
                else if (player is Player)
                {
                    DisplayBuildInstructions(2);
                }
            });
        }

        public void DisplayShotMessage(int i)
        {
            switch (i)
            {
                case 0:
                    AnsiConsole.Write(new Markup("[red]Trafiony![/]"));
                    AnsiConsole.WriteLine("\nNaciśnij dowolny klawisz, aby oddać kolejny strzał.");
                    break;
                case 1:
                    AnsiConsole.WriteLine("Nieprawidłowe współrzędne. Wprowadź literę (A-J) i cyfrę (1-10).");
                    AnsiConsole.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować.");
                    break;
                case 2:
                    AnsiConsole.WriteLine("Pierwsza współrzędna musi być literą od A do J.");
                    AnsiConsole.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować.");
                    break;
                case 3:
                    AnsiConsole.WriteLine("Druga współrzędna musi być liczbą od 1 do 10.");
                    AnsiConsole.WriteLine("\nNaciśnij dowolny klawisz, aby kontynuować.");
                    break;
                case 4:
                    AnsiConsole.Write(new Markup("[yellow]Pudło![/]"));
                    AnsiConsole.WriteLine("\nNaciśnij dowolny klawisz, aby zmienić turę!");
                    break;
            }
        }

        private void PlaceShipForPlayer(Player player, int shipType)
        {
            int length = shipType == 1 ? 5 : shipType == 2 ? 4 : 3;

            if (!player.CanAddShipOfLength(length))
            {
                Console.WriteLine($"Cannot add more ships of length {length}.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            while (true)
            {
                Console.Clear();
                player.UpdatePlaceholderShip(length);
                DisplayBuilderMap(player.GetFleet().getParts(), player.CurrentPlaceholderShip);
                DisplayCurrentPlayer(player.name);

                ConsoleKeyInfo key = GetInput();
                if (player.HandleShipPlacementInput(key, length))
                    break;
            }
        }
        public string MainMenu()
        {
            var choices = new List<string> {
                        "Player vs Computer",
                        "Options", "Achievements", "Ranking",
                        "History", "Exit"
                    };
            if (GameAchievements.GetAchievement("PvP").IsCompleted)
                choices.Insert(1, "Simulation");
            if (GameAchievements.GetAchievement("PvE").IsCompleted)
                choices.Insert(1,"Player vs Player");
            

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [green]option[/]")
                    .PageSize(10)
                    .AddChoices(choices));
            return option;
        }
        public void DisplayOptions()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(new Markup($"Current Map color is [{_assetManager.MapColor}]{_assetManager.MapColor}[/]\n"));

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [green]option[/] to change")
                        .PageSize(10)
                        .AddChoices(new[] {
                        "Map color", "Back"
                        }));
                switch (option)
                {
                    case "Map color":
                        ChangeColor();
                        break;
                    case "Back":
                        return;
                }
            }
        }
        public void ChangeColor()
        {
            var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [green]color[/]")
                        .PageSize(10)
                        .AddChoices(new[] {
                        "white", "blue", "red", "yellow","purple"
                        }));
            _assetManager.MapColor = option;
        }

        public void DisplayHistory()
        {
            var games = GameHistory.ReadGameHistory();

            var table = new Table();
            table.AddColumn("Date");
            table.AddColumn("Player 1");
            table.AddColumn("Player 2");
            table.AddColumn("P1 Shots");
            table.AddColumn("P2 Shots");
            table.AddColumn("Winner");

            foreach (var game in games)
            {
                table.AddRow(
                    game.GameDate.ToString("dd/MM/yyyy HH:mm"),
                    game.Player1Name,
                    game.Player2Name,
                    game.Player1Shots.ToString(),
                    game.Player2Shots.ToString(),
                    game.Winner
                );
            }

            Console.Clear();
            AnsiConsole.Write(table);
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
            Console.Clear();
        }

        public void DisplayGameSummary(IPlayer player1, IPlayer player2, IPlayer winner)
        {
            Console.Clear();
            AnsiConsole.Write(new Markup($"[green]GAME OVER! Winner: {winner.name}[/]"));
            Console.WriteLine("\n\nPress any key to see players' fleets...");
            Console.ReadKey(true);

            Console.Clear();
            Console.WriteLine($"{player1.name}'s fleet:");
            Console.WriteLine();
            DrawBaseGrid(0);
            DisplayShipParts(player1.GetFleet().getParts(), 0);
            foreach (var shot in player2.GetShots())
            {
                Console.SetCursorPosition(shot.getPosition().x * 5 + 6, shot.getPosition().y * 2 + 2);
                shot.display();
            }
            Console.SetCursorPosition(0, 22);
            Console.WriteLine("\n\nPress any key to see next player's fleet...");
            Console.ReadKey(true);

            Console.Clear();
            Console.WriteLine($"{player2.name}'s fleet:");
            Console.WriteLine();
            DrawBaseGrid(0);
            DisplayShipParts(player2.GetFleet().getParts(), 0);
            foreach (var shot in player1.GetShots())
            {
                Console.SetCursorPosition(shot.getPosition().x * 5 + 6, shot.getPosition().y * 2 + 2);
                shot.display();
            }
            Console.SetCursorPosition(0, 22);
            Console.WriteLine("\n\nPress any key to return to menu...");
            Console.ReadKey(true);
        }

        public void DisplayRanking()
        {
            var games = GameHistory.ReadGameHistory();
            
            var playerStats = games.SelectMany(g => new[] 
            {
                (Name: g.Player1Name, Shots: g.Player1Shots, IsWinner: g.Winner == g.Player1Name),
                (Name: g.Player2Name, Shots: g.Player2Shots, IsWinner: g.Winner == g.Player2Name)
            })
            .GroupBy(p => p.Name)
            .Select(g => new
            {
                PlayerName = g.Key,
                GamesPlayed = g.Count(),
                Wins = g.Count(p => p.IsWinner),
                AverageShots = g.Average(p => p.Shots),
                WinRate = (double)g.Count(p => p.IsWinner) / g.Count() * 100
            })
            .OrderByDescending(p => p.Wins)
            .ToList();

            var table = new Table();
            table.Title = new TableTitle("[yellow]Player Rankings[/]");
            table.AddColumn("Rank");
            table.AddColumn("Player Name");
            table.AddColumn("Games Played");
            table.AddColumn("Wins");
            table.AddColumn("Win Rate %");
            table.AddColumn("Avg. Shots");

            int rank = 1;
            foreach (var stat in playerStats)
            {
                table.AddRow(
                    rank.ToString(),
                    stat.PlayerName,
                    stat.GamesPlayed.ToString(),
                    stat.Wins.ToString(),
                    $"{stat.WinRate:F1}%",
                    $"{stat.AverageShots:F1}"
                );
                rank++;
            }

            Console.Clear();
            AnsiConsole.Write(table);
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
            Console.Clear();
        }
        public void DisplayAchievements()
        {
            var achievements = GameAchievements.GetAllAchievements();

            var table = new Table();
            table.AddColumn("Achievement");
            table.AddColumn("Is Complete");

            foreach (var achievement in achievements)
            {
                table.AddRow(achievement.Description,achievement.IsCompleted.ToString());
            }

            Console.Clear();
            AnsiConsole.Write(table);
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
