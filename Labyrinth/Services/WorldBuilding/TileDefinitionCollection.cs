using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    class TileDefinitionCollection
        {
        public Rectangle Area { get; set; }
        public readonly Dictionary<char, TileDefinition> Definitions = new Dictionary<char, TileDefinition>();

        public void Add([NotNull] TileDefinition td)
            {
            if (td == null) throw new ArgumentNullException(nameof(td));
            this.Definitions.Add(td.Symbol, td);
            }

        public string GetDefaultFloor()
            {
            var defaultFloorDef = this.Definitions.Values.OfType<TileFloorDefinition>().SingleOrDefault()
                                  ?? this.Definitions.Values.OfType<TileFloorDefinition>().Single();
            return defaultFloorDef.TextureName;
            }

        public static TileDefinitionCollection FromXml(XmlNodeList tiledefs)
            {
            var result = new TileDefinitionCollection();
            foreach (XmlElement tiledef in tiledefs)
                {
                string symbol = tiledef.GetAttribute("Symbol");
                switch (tiledef.Name)
                    {
                    case "Wall":
                        {
                        string texture = tiledef.GetAttribute("Tile");
                        var td = new TileWallDefinition(symbol, texture);
                        result.Add(td);
                        break;
                        }
                    case "Floor":
                        {
                        string texture = tiledef.GetAttribute("Tile");
                        bool.TryParse(tiledef.GetAttribute("IsDefault"), out var isDefault);
                        var td = new TileFloorDefinition(symbol, texture, isDefault);
                        result.Add(td);
                        break;
                        }
                    case "Object":
                        {
                        string description = tiledef.GetAttribute("Description");
                        var td = new TileObjectDefinition(symbol, description);
                        result.Add(td);
                        break;
                        }
                    }
                }
            
            var countOfDefaultFloorTiles = result.Definitions.Values.OfType<TileFloorDefinition>().Count(item => item.IsDefault);
            if (countOfDefaultFloorTiles > 1)
                throw new InvalidOperationException("More than one floor tile is marked as the default.");
            if (countOfDefaultFloorTiles == 0 && result.Definitions.Values.OfType<TileFloorDefinition>().Count() > 1)
                throw new InvalidOperationException("There are more than one floor tiles, and none of them is marked as the default.");
            return result;
            }
        }
    }
