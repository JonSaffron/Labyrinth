using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    internal class TileDefinitionCollection : IHasArea
        {
        public Rectangle Area { get; set; }
        private readonly Dictionary<char, TileDefinition> _definitions = new Dictionary<char, TileDefinition>();

        private void Add(TileDefinition td)
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

        public static TileDefinitionCollection FromXml(XmlNodeList tileDefinitions)
            {
            var result = new TileDefinitionCollection();
            foreach (XmlElement tileDef in tileDefinitions)
                {
                string symbol = tileDef.GetAttribute("Symbol");
                switch (tileDef.Name)
                    {
                    case "Wall":
                        {
                        string texture = tileDef.GetAttribute("Tile");
                        var td = new TileWallDefinition(symbol, texture);
                        result.Add(td);
                        break;
                        }
                    case "Floor":
                        {
                        string texture = tileDef.GetAttribute("Tile");
                        bool.TryParse(tileDef.GetAttribute("IsDefault"), out var isDefault);
                        var td = new TileFloorDefinition(symbol, texture, isDefault);
                        result.Add(td);
                        break;
                        }
                    case "Object":
                        {
                        string description = tileDef.GetAttribute("Description");
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
