using System.Collections.Generic;
using System.Xml;

namespace Battleship.Pattern
{
    public class AssetManager
    {
        private Dictionary<Tuple<ShipType, bool>, string> m_Skins;
        private const int SHIP_TYPES = 4;
        private const string SKIN_PATH = "skins.txt";
        private static readonly AssetManager sInstance = new AssetManager();

        public string MapColor = "red";

        public AssetManager()
        {
            m_Skins = new Dictionary<Tuple<ShipType, bool>, string>();
        }

        public static AssetManager Instance
        {
            get { return sInstance; }
        }

        public string GetSkin(ShipType type, bool custom = false)
        {
            if (m_Skins.TryGetValue(Tuple.Create(type, custom), out var skin))
                return skin;
            
            return AddSkin(type, custom);
        }

        private string AddSkin(ShipType type, bool custom = false)
        {
            // Statki domyślne będą w liniach [0,3], a statki użytkownika w liniach [4,7]
            int lineNumber = ((custom ? 1 : 0) * SHIP_TYPES)  + ((int)type - 2);

            try
            {
                int currentLine = 0;

                foreach (var line in File.ReadLines(SKIN_PATH))
                {
                    if (currentLine == lineNumber)
                    {
                        m_Skins.Add(Tuple.Create(type, custom), line);
                        return line;
                    }

                    currentLine++;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file was not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return "";
        }

    }
}
