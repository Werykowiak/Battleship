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
        private Boolean isCustom;

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

        // Method to set the ship's custom skin
        public ShipBuilder SetCustomSkin(Boolean isCustom)
        {
            this.isCustom = isCustom;
            return this;
        }

        public Ship Build()
        {
            List<IShipInterface> shipParts = new List<IShipInterface>();

            ShipType shipType = (ShipType)_length;

            string skin = _assetManager.GetSkin(shipType, isCustom);

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
                else
                {
                    partPosition = new Vector2i(
                        _startPosition.x,
                        _startPosition.y + i
                    );
                }

                shipParts.Add(new ShipPart(partPosition.x, partPosition.y, partRepresentation));
            }

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