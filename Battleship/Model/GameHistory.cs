using System.Text.Json;

namespace Battleship.Model
{
    public static class GameHistory
    {
        private const string HISTORY_FILE = "gameHistory.json";

        public static void SaveGame(GameRecord game)
        {
            var games = ReadGameHistory().ToList();
            games.Add(game);
            File.WriteAllText(HISTORY_FILE, JsonSerializer.Serialize(games));
        }

        public static GameRecord[] ReadGameHistory()
        {
            if (!File.Exists(HISTORY_FILE))
                return Array.Empty<GameRecord>();

            string json = File.ReadAllText(HISTORY_FILE);
            return JsonSerializer.Deserialize<GameRecord[]>(json) ?? Array.Empty<GameRecord>();
        }
    }
}
