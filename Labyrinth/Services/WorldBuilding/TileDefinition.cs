using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.WorldBuilding
    {
    public abstract class TileDefinition
        {
        public readonly char Symbol;

        protected TileDefinition([NotNull] string symbol)
            {
            if (string.IsNullOrEmpty(symbol) || symbol.Length != 1)
                throw new InvalidOperationException();
            this.Symbol = symbol[0];
            }
        }

    public class TileWallDefinition : TileDefinition
        {
        public readonly string TextureName;

        public TileWallDefinition([NotNull] string symbol, [NotNull] string textureName) : base(symbol)
            {
            this.TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            }
        }

    public class TileFloorDefinition : TileDefinition
        {
        public readonly string TextureName;
        public readonly bool IsDefault;

        public TileFloorDefinition([NotNull] string symbol, [NotNull] string textureName, bool isDefault) : base(symbol)
            {
            this.TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            this.IsDefault = isDefault;
            }
        }

    public class TileObjectDefinition : TileDefinition
        {
        public readonly string Description;

        public TileObjectDefinition([NotNull] string symbol, [NotNull] string description) : base(symbol)
            {
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            }
        }
    }
