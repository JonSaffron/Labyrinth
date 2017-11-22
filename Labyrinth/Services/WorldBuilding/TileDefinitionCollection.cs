using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    class TileDefinitionCollection : IHasArea
        {
        public Rectangle Area { get; set; }
        private readonly Dictionary<char, TileDefinition> _definitions = new Dictionary<char, TileDefinition>();

        public void Add([NotNull] TileDefinition td)
            {
            if (td == null) throw new ArgumentNullException(nameof(td));
            this._definitions.Add(td.Symbol, td);
            }

        public string GetDefaultFloor()
            {
            var defaultFloorDef = this._definitions.Values.OfType<TileFloorDefinition>().SingleOrDefault()
                                  ?? this._definitions.Values.OfType<TileFloorDefinition>().Single();
            return defaultFloorDef.TextureName;
            }

        public TileDefinition this[char symbol]
            {
            get
                {
                if (!this._definitions.TryGetValue(symbol, out var result))
                    {
                    string text = $"Symbol {symbol} is not defined in area {this.Area}";
                    throw new InvalidOperationException(text);
                    }
                return result;
                }   
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
            
            var countOfDefaultFloorTiles = result._definitions.Values.OfType<TileFloorDefinition>().Count(item => item.IsDefault);
            if (countOfDefaultFloorTiles > 1)
                throw new InvalidOperationException("More than one floor tile is marked as the default.");
            if (countOfDefaultFloorTiles == 0 && result._definitions.Values.OfType<TileFloorDefinition>().Count() > 1)
                throw new InvalidOperationException("There are more than one floor tiles, and none of them is marked as the default.");
            return result;
            }
        }
    }
