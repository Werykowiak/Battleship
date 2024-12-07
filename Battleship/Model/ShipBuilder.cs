using Battleship.Pattern;
using System;

namespace Battleship.Model
{
    public class ShipBuilder
    {
        private int _length;
        private Orientation _orientation;
        private Vector2i _startPosition;
        private AssetManager _assetManager;

        public ShipBuilder()
        {
            _assetManager = AssetManager.Instance;
        }

        // Method to set the ship length
        public ShipBuilder SetLength(int length)
        {
            _length = length;
            return this;
        }

        // Method to set the ship orientation
        public ShipBuilder SetOrientation(Orientation orientation)
        {
            _orientation = orientation;
            return this;
        }

        // Method to set the ship's starting position
        public ShipBuilder SetStartPosition(Vector2i position)
        {
            _startPosition = position;
            return this;
        }

        public Ship Build()
        {
            List<ShipPart> shipParts = new List<ShipPart>();

            // Determine ship type based on length
            ShipType shipType = (ShipType)_length;

            // Get the skin for the ship (assuming custom = true for player ships)
            string skin = _assetManager.GetSkin(shipType, true);

            // Create ship parts based on orientation and length
            for (int i = 0; i < _length; i++)
            {
                Vector2i partPosition;
                char partRepresentation = skin[i];

                if (_orientation == Orientation.Horizontal)
                {
                    partPosition = new Vector2i(
                        _startPosition.x + i,
                        _startPosition.y
                    );
                }
                else // Vertical orientation
                {
                    partPosition = new Vector2i(
                        _startPosition.x,
                        _startPosition.y + i
                    );
                }

                // Create a ShipPart for each position with the corresponding skin character
                shipParts.Add(new ShipPart(partPosition.x, partPosition.y, partRepresentation));
            }

            // Create and return a new Ship with the generated parts
            return new Ship(shipParts);
        }
    }

    // Enum for orientation
    public enum Orientation
    {
        Horizontal,
        Vertical
    }
}