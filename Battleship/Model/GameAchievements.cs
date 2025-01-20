using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Battleship.Model
{
    public static class GameAchievements
    {
        private const string ACHIEVEMENT_FILE = "GameAchievements.json";

        public static Achievement GetAchievement(string achievenmentName)
        {
            if (!File.Exists(ACHIEVEMENT_FILE))
                return null;

            string json = File.ReadAllText(ACHIEVEMENT_FILE);
            var achievements = JsonSerializer.Deserialize<Achievement[]>(json) ?? Array.Empty<Achievement>();

            return achievements.First(p => p.Name == achievenmentName);
        }
        public static bool UpdateAchievement(string achievenmentName)
        {
            if (!File.Exists(ACHIEVEMENT_FILE))
                return false;

            string json = File.ReadAllText(ACHIEVEMENT_FILE);
            var achievements = JsonSerializer.Deserialize<Achievement[]>(json) ?? Array.Empty<Achievement>();

            var achievement = achievements.First(p => p.Name == achievenmentName);
            achievement.IsCompleted = true;

            json = JsonSerializer.Serialize(achievements, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ACHIEVEMENT_FILE, json);
            return true;
        }
        public static Achievement[] GetAllAchievements()
        {
            if (!File.Exists(ACHIEVEMENT_FILE))
                return Array.Empty<Achievement>();

            string json = File.ReadAllText(ACHIEVEMENT_FILE);
            return JsonSerializer.Deserialize<Achievement[]>(json) ?? Array.Empty<Achievement>();
        }
    }
}
