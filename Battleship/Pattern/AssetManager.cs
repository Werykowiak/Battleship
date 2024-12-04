using System.Collections.Generic;

namespace Battleship.Pattern
{
    public class AssetManager
    {
        private Dictionary<string, string> m_Skins;
        private static readonly AssetManager sInstance = new AssetManager();

        public AssetManager()
        {
            m_Skins = new Dictionary<string, string>();
        }

        public static AssetManager Instance
        {
            get { return sInstance; }
        }

        public string GetSkin(string filename)
        {
            if (m_Skins.TryGetValue(filename, out var skin))
                return skin;
            else
                throw new KeyNotFoundException($"Skin '{filename}' not found.");
        }

        public void AddSkin(string filename, string skin)
        {
            if (!m_Skins.ContainsKey(filename))
                m_Skins[filename] = skin;
        }
    }
}
