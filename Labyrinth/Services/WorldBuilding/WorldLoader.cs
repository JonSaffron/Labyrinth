﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using JetBrains.Annotations;

namespace Labyrinth.Services.WorldBuilding
    {
    // todo is there a way to reduce the number of times the world layout is examined?
    public class WorldLoader : IWorldLoader
        {
        private XmlElement _xmlRoot;
        private XmlNamespaceManager _xnm;
        private List<WorldArea> _worldAreas;
        public TilePos WorldSize { get; private set; }

        public void LoadWorld(string levelName)
            {
            string worldDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Worlds");

            string pathToWorld = worldDirectory + "/" + levelName;
            if (!File.Exists(pathToWorld))
                throw new ArgumentOutOfRangeException(pathToWorld);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(pathToWorld);

            var validator = new WorldValidator();
            var pathToXsd = worldDirectory + "/WorldSchema.xsd";
            validator.Validate(xmlDoc.OuterXml, pathToXsd);

            this._xmlRoot = xmlDoc.DocumentElement;
            if (this._xmlRoot == null)
                throw new InvalidOperationException("Empty xml document.");
            this._xnm = new XmlNamespaceManager(xmlDoc.NameTable);
            this._xnm.AddNamespace("ns", "http://JonSaffron/Labyrinth");

            int width = int.Parse(this._xmlRoot.GetAttribute("Width"));
            int height = int.Parse(this._xmlRoot.GetAttribute("Height"));
            this.WorldSize = new TilePos(width, height);

            this._worldAreas = LoadAreas();
            }

        public Tile[,] GetFloorTiles()
            {
            var result = GetFloorLayout();
            return result;
            }

        public bool RestartInSameRoom
            {
            get
                {
                var text = this._xmlRoot.GetAttribute("RestartInSameRoom");
                if (string.IsNullOrWhiteSpace(text))
                    return false;
                bool result = bool.Parse(text);
                return result;
                }
            }

        public Dictionary<int, PlayerStartState> GetPlayerStartStates()
            {
            var result = this._worldAreas.Where(item => item.Id.HasValue)
                .ToDictionary(key => key.Id.GetValueOrDefault(), value => value.PlayerStartState);
            return result;
            }

        public void AddGameObjects(GameState gameState)
            {
            var layout = GetLayout();

            var x = new ProcessGameObjects(gameState);
            x.AddWalls(this._worldAreas, layout);
            x.AddPlayerAndStartPositions(this._worldAreas);

            var objects = this._xmlRoot.SelectNodes(@"ns:Objects/ns:*", this._xnm);
            if (objects != null)
                {
                var objectList = objects.Cast<XmlElement>();
                x.AddGameObjects(objectList);
                }

            ValidateGameState(gameState);

            x.AddMonstersFromRandomDistribution(this._worldAreas);
            x.AddFruit(this._worldAreas);
            }

        private string[] GetLayout()
            {
            var layoutDef = (XmlElement)this._xmlRoot.SelectSingleNode("ns:Layout", this._xnm);
            if (layoutDef == null)
                throw new InvalidOperationException("No Layout element found.");
            var layout = layoutDef.InnerText;
            var result = layout.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
            return result;
            }

        private List<WorldArea> LoadAreas()
            {
            var areas = this._xmlRoot.SelectSingleNode("ns:Areas", this._xnm);
            if (areas == null)
                throw new InvalidOperationException();
            var result = new List<WorldArea>();
            XmlNodeList areaList = areas.SelectNodes("ns:Area", this._xnm);
            if (areaList == null)
                throw new InvalidOperationException();
            foreach (XmlElement a in areaList)
                {
                var wa = new WorldArea(a, this._xnm);
                if (wa.TileDefinitions != null && wa.TileDefinitions.Count != 0)
                    {
                    WorldArea intersectingArea =
                    (from WorldArea r in result
                        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                        where r.Area.Intersects(wa.Area) && r.TileDefinitions != null && r.TileDefinitions.Count != 0
                        select r).FirstOrDefault();
                    if (intersectingArea != null)
                        throw new InvalidOperationException(
                            $"The area {wa.Area} intersects with another area {intersectingArea.Area} (this is a problem because there are multiple tile definitions).");
                    }

                result.Add(wa);
                }
            if (result.Count(wa => wa.IsInitialArea) != 1)
                throw new InvalidOperationException("One and only one world area should be marked as the initial area.");
            return result;
            }

        private Tile[,] GetFloorLayout()
            {
            var layout = GetLayout();
            ValidateLayout(layout);

            var result = new Tile[this.WorldSize.X, this.WorldSize.Y];
            var spriteLibrary = GlobalServices.SpriteLibrary;
            foreach (WorldArea wa in this._worldAreas)
                {
                if (wa.TileDefinitions == null || wa.TileDefinitions.Count == 0)
                    continue;

                string defaultFloorName = GetDefaultFloor(wa.TileDefinitions.Values);
                foreach (TilePos p in wa.Area.PointsInside())
                    {
                    char symbol = layout[p.Y][p.X];
                    if (!wa.TileDefinitions.TryGetValue(symbol, out var td))
                        {
                        string text = $"Don't know what symbol {symbol} indicates in world area {wa.Id}";
                        throw new InvalidOperationException(text);
                        }

                    var textureName =
                        td is TileFloorDefinition definition
                            ? definition.TextureName
                            : defaultFloorName;
                    var floor = spriteLibrary.GetSprite("Tiles/" + textureName);

                    if (!wa.Id.HasValue)
                        throw new InvalidOperationException("Area with tile definitions needs an id.");
                    result[p.X, p.Y] = new Tile(floor, wa.Id.Value);
                    }
                }
            return result;
            }

        private static string GetDefaultFloor([NotNull] ICollection<TileDefinition> tileDefinitions)
            {
            var defaultFloorDef = tileDefinitions.OfType<TileFloorDefinition>().SingleOrDefault()
                                  ?? tileDefinitions.OfType<TileFloorDefinition>().Single();
            return defaultFloorDef.TextureName;
            }

        private void ValidateLayout(string[] lines)
            {
            TilePos size = this.WorldSize;
            int countOfLines = lines.GetLength(0);
            if (countOfLines != size.Y)
                throw new InvalidOperationException($"The world layout has {countOfLines} lines whilst the Height property equals {size.Y}. The two should match.");

            for (int y = 0; y < size.Y; y++)
                {
                var lineLength = lines[y].Length;
                if (lineLength != size.X)
                    throw new InvalidOperationException($"Line {y} has {lineLength} characters, whilst the Width property equals {size.X}. The two should match.");
                }
            }

        private void ValidateGameState(GameState gameState)
            {
            var tileUsage = BuildTileUsageByMap();

            var issues = new List<string>();
            var cy = this.WorldSize.Y;
            var cx = this.WorldSize.X;
            for (int y = 0; y < cy; y++)
                {
                for (int x = 0; x < cx; x++)
                    {
                    var tp = new TilePos(x, y);
                    var items = gameState.GetItemsOnTile(tp).ToList();
                    TileUsage t = tileUsage[tp];
                    if (t.TileTypeByMap == TileTypeByMap.Object && !items.Any())
                        issues.Add(tp + ": Map had tile marked as occupied by an object " + t.Description + ", but nothing is there.");
                    else if (t.TileTypeByMap == TileTypeByMap.Floor && items.Any())
                        issues.Add(tp + ": Map had tile marked as unoccupied, but contains " + items.Count + " item(s): " + string.Join(", ", items.Select(item => item.GetType().Name)) + ".");
                    }
                }

            var message = string.Join("\r\n", issues);
            Trace.WriteLine(message);
            var dr = MessageBox.Show(message, "Warnings", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dr == DialogResult.Cancel)
                throw new InvalidOperationException(message);
            }

        private Dictionary<TilePos, TileUsage> BuildTileUsageByMap()
            {
            var layoutDef = (XmlElement) this._xmlRoot.SelectSingleNode("ns:Layout", this._xnm);
            if (layoutDef == null)
                throw new InvalidOperationException("No Layout element found.");
            var layout = layoutDef.InnerText;
            var lines = layout.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();

            var result = new Dictionary<TilePos, TileUsage>();
            foreach (WorldArea wa in this._worldAreas)
                {
                var tileDefs = wa.TileDefinitions;
                if (tileDefs == null || tileDefs.Count == 0)
                    continue;

                foreach (TilePos p in wa.Area.PointsInside())
                    {
                    char symbol = lines[p.Y][p.X];
                    if (!tileDefs.TryGetValue(symbol, out var td))
                        {
                        string text = $"Don't know what symbol {symbol} indicates in world area {(wa.Id.HasValue ? (object) wa.Id.Value : "(no number)")}";
                        throw new InvalidOperationException(text);
                        }
                    if (td is TileWallDefinition)
                        {
                        result.Add(p, TileUsage.Wall(symbol));
                        }
                    else if (td is TileFloorDefinition)
                        {
                        result.Add(p, TileUsage.Floor(symbol));
                        }
                    else if (td is TileObjectDefinition objectDef)
                        {
                        result.Add(p, TileUsage.Object(symbol, objectDef.Description));
                        }
                    else
                        {
                        throw new InvalidOperationException();
                        }
                    }
                }
            return result;
            }
        }
    }
