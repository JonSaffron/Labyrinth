using System;

namespace Labyrinth.Services.WorldBuilding
    {
    public struct TileDefinition
        {
        public readonly char Symbol;
        public readonly TileTypeByMap TileTypeByMap;
        public readonly string TextureName;
        
        public TileDefinition(string symbol, string tileTypeByMap, string textureName)
            {
            if (string.IsNullOrEmpty(symbol) || symbol.Length != 1)
                throw new InvalidOperationException();
            this.Symbol = symbol[0];

            if (!Enum.TryParse(tileTypeByMap, true, out this.TileTypeByMap))
                throw new InvalidOperationException();

            this.TextureName = textureName;
            }
        }
    }
