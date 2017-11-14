using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using JetBrains.Annotations;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public class WorldLoader : IWorldLoader
        {
        private XmlElement _xmlRoot;
        private XmlNamespaceManager _xnm;
        private List<WorldArea> _worldAreas;
        
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
            this._xnm = new XmlNamespaceManager(xmlDoc.NameTable);
            this._xnm.AddNamespace("ns", "http://JonSaffron/Labyrinth");
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

        public void AddGameObjects(GameState gameState)
            {
            var x = new ProcessGameObjects(this, gameState);
            x.AddGameObjects();
            }

        /// <summary>
        /// Size of World measured in tiles.
        /// </summary>
        private TilePos Size
            {
            get 
                { 
                int width = int.Parse(this._xmlRoot.GetAttribute("Width"));
                int height = int.Parse(this._xmlRoot.GetAttribute("Height"));
                var result = new TilePos(width, height);
                return result;
                }
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
                        where r.Area.Intersects(wa.Area) && r.TileDefinitions != null && r.TileDefinitions.Count != 0
                        select r).FirstOrDefault();
                    if (intersectingArea != null)
                        throw new InvalidOperationException($"The area {wa.Area} intersects with another area {intersectingArea.Area} (this is a problem because there are multiple tile definitions).");
                    }
                
                result.Add(wa);
                }
            return result;
            }

        public Dictionary<int, PlayerStartState> GetPlayerStartStates()
            {
            var result = this._worldAreas.Where(item => item.Id.HasValue).ToDictionary(key => key.Id.GetValueOrDefault(), value => value.PlayerStartState);
            return result;
            }

        private Tile[,] GetFloorLayout()
            {
            var layout = (XmlElement) this._xmlRoot.SelectSingleNode("ns:Layout", this._xnm);
            if (layout == null)
                throw new InvalidOperationException("No Layout element found.");
            
            var lines = layout.InnerText.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
            ValidateLayout(lines);
               
            var size = this.Size;
            var result = new Tile[size.X, size.Y];
            var spriteLibrary = GlobalServices.SpriteLibrary;
            foreach (WorldArea wa in this._worldAreas)
                {
                if (wa.TileDefinitions == null || wa.TileDefinitions.Count == 0)
                    continue;
                    
                string defaultFloorName = GetDefaultFloor(wa.TileDefinitions.Values);
                foreach (TilePos p in wa.Area.PointsInside())
                    {
                    char symbol = lines[p.Y][p.X];
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
            TilePos size = this.Size;
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

        internal class ProcessGameObjects
            {
            private readonly WorldLoader _wl;
            private readonly GameState _gameState;

            public ProcessGameObjects(WorldLoader wl, GameState gameState)
                {
                this._wl = wl;
                this._gameState = gameState;
                }

            public void AddGameObjects()
                {
                var tileUsage = new TileUsage[this._wl.Size.X, this._wl.Size.Y];
                this.AddWalls(ref tileUsage);
                this.AddPlayerAndStartPositions();

                var objects = this._wl._xmlRoot.SelectNodes(@"ns:Objects/ns:*", this._wl._xnm);
                var objectList = objects?.Cast<XmlElement>() ?? Enumerable.Empty<XmlElement>();
                var objectCreationHandlers = BuildMapForObjectCreation();
                foreach (XmlElement definition in objectList)
                    {
                    if (!objectCreationHandlers.TryGetValue(definition.LocalName, out var action))
                        {
                        throw new InvalidOperationException("Unknown object type: " + definition.LocalName);
                        }
                    action(definition);
                    }

                AddMonstersFromRandomDistribution(ref tileUsage);
                AddFruit(ref tileUsage);

                ValidateGameState(ref tileUsage);
                }

            private void AddPlayerAndStartPositions()
                {
                if (this._wl._worldAreas.Count(wa => wa.IsInitialArea) != 1)
                    throw new InvalidOperationException("One and only one world area should be marked as the initial area.");
                foreach (var wa in this._wl._worldAreas)
                    {
                    if (wa.IsInitialArea)
                        {
                        if (wa.PlayerStartState == null)
                            throw new InvalidOperationException("Initial world area should have a player start state.");
                        this._gameState.AddPlayer(wa.PlayerStartState.Position.ToPosition(), wa.PlayerStartState.Energy, wa.Id);
                        }
                    else if (wa.PlayerStartState != null)
                        {
                        this._gameState.AddTileReservation(wa.PlayerStartState.Position.ToPosition());
                        }
                    }
                }

            private void AddWalls(ref TileUsage[,] tileUsage)
                {
                var layoutDef = (XmlElement)this._wl._xmlRoot.SelectSingleNode("ns:Layout", this._wl._xnm);
                if (layoutDef == null)
                    throw new InvalidOperationException("No Layout element found.");
                var layout = layoutDef.InnerText;
                var lines = layout.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
                
                foreach (WorldArea wa in this._wl._worldAreas)
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
                        if (td is TileWallDefinition wall)
                            { 
                            tileUsage[p.X, p.Y] = TileUsage.Wall(symbol);
                            this._gameState.AddWall(p.ToPosition(), "Tiles/" + wall.TextureName);
                            }
                        else if (td is TileFloorDefinition)
                            {
                            tileUsage[p.X, p.Y] = TileUsage.Floor(symbol);
                            }
                        else if (td is TileObjectDefinition objectDef)
                            {
                            tileUsage[p.X, p.Y] = TileUsage.Object(symbol, objectDef.Description);  
                            }
                        else
                            { 
                            throw new InvalidOperationException();
                            }
                        }
                    }
                }

            private void ValidateGameState(ref TileUsage[,] tileUsage)
                {
                var tcv = new TileContentValidator();
                var issues = new List<string>();
                var cy = tileUsage.GetLength(1);
                var cx = tileUsage.GetLength(0);
                for (int y = 0; y < cy; y++)
                    {
                    for (int x = 0; x < cx; x++)
                        {
                        var tp = new TilePos(x, y);
                        var items = this._gameState.GetItemsOnTile(tp).ToList();
                        if (!tcv.IsListOfObjectsValid(items, out string reason))
                            issues.Add(tp + ": " + reason);
                        TileUsage t = tileUsage[x, y];
                        if (t.TileTypeByMap == TileTypeByMap.Object && !items.Any())
                            issues.Add(tp + ": Map had tile marked as occupied by an object " + t.Description + ", but nothing is there.");
                        else if (t.TileTypeByMap == TileTypeByMap.Floor && !t.IsUsedByRandomDistribution && items.Any())
                            issues.Add(tp + ": Map had tile marked as unoccupied, but contains " + items.Count + " item(s).");
                        }
                    }

                var message = string.Join("\r\n", issues);
                Trace.WriteLine(message);
                var dr = MessageBox.Show(message, "Warnings", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    throw new InvalidOperationException(message);
                }

            private void AddMonster(XmlElement mdef)
                {
                MonsterDef md = MonsterDef.FromXml(mdef);
                var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
                md.Position = tilePos.ToPosition();
                this._gameState.CreateMonster(md);
                }

            private void AddCrystal(XmlElement cdef)
                {
                var id = int.Parse(cdef.GetAttribute("Id"));
                var tilePos = new TilePos(int.Parse(cdef.GetAttribute("Left")), int.Parse(cdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var score = int.Parse(cdef.GetAttribute("Score"));
                var energy = int.Parse(cdef.GetAttribute("Energy"));
                this._gameState.AddCrystal(position, id, score, energy);
                }

            private void AddBoulder(XmlElement bdef)
                {
                var tilePos = new TilePos(int.Parse(bdef.GetAttribute("Left")), int.Parse(bdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                this._gameState.AddBoulder(position);
                }

            private void AddForceFields(XmlElement fdef)
                {
                int crystalRequired = int.Parse(fdef.GetAttribute("CrystalRequired"));
                Rectangle r = WorldArea.GetRectangleFromDefinition(fdef);
                foreach (var tp in r.PointsInside())
                    {
                    this._gameState.AddForceField(tp.ToPosition(), crystalRequired);
                    }
                }

            private void AddCrumblyWall(XmlElement wdef)
                {
                var tilePos = new TilePos(int.Parse(wdef.GetAttribute("Left")), int.Parse(wdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var energy = int.Parse(wdef.GetAttribute("Energy"));
                var textureName = wdef.GetAttribute("Texture");
                this._gameState.AddCrumblyWall(position, "Tiles/" + textureName, energy);
                }

            private void AddMonstersFromRandomDistribution(ref TileUsage[,] tileUsage)
                {
                var rnd = GlobalServices.Randomess;
                var dists = 
                    this._wl._worldAreas.Where(wa => wa.RandomMonsterDistribution != null)
                        .Select(wa => new { wa.Area, Dist = wa.RandomMonsterDistribution});
                foreach (var item in dists)
                    {
                    for (int i = 0; i < item.Dist.CountOfMonsters; i++)
                        {
                        var monsterIndex = rnd.DiceRoll(item.Dist.DiceRoll);
                        var monsterDef = item.Dist.Templates[monsterIndex];

                        TilePos tp = GetFreeTile(item.Area);
                        monsterDef.Position = tp.ToPosition();
                        this._gameState.CreateMonster(monsterDef);

                        tileUsage[tp.X, tp.Y].SetUsedByRandomDistribution();
                        i++;
                        }
                    }
                }

            private void AddFruit(ref TileUsage[,] tileUsage)
                {
                var fruitDefinitions =
                    this._wl._worldAreas.Where(wa => wa.FruitDefinitions != null)
                        .SelectMany(wa => wa.FruitDefinitions.Values, (wa, fd) => new {wa.Area, FruitDefinition = fd});
                foreach (var item in fruitDefinitions)
                    {
                    for (int i = 0; i < item.FruitDefinition.FruitQuantity;)
                        {
                        TilePos tp = GetFreeTile(item.Area);
                        Vector2 position = tp.ToPosition();
                        this._gameState.AddFruit(position, item.FruitDefinition.FruitType, item.FruitDefinition.Energy);

                        tileUsage[tp.X, tp.Y].SetUsedByRandomDistribution();
                        i++;
                        }
                    }
                }

            private TilePos GetFreeTile(Rectangle area)
                {
                var rnd = GlobalServices.Randomess;
                while (true)
                    {
                    var tp = new TilePos(area.X + rnd.Next(area.Width), area.Y + rnd.Next(area.Height));
                    if (!this._gameState.GetItemsOnTile(tp).Any())
                        {
                        return tp;
                        }
                    }
                }

            private Dictionary<string, Action<XmlElement>> BuildMapForObjectCreation()
                {
                var result = new Dictionary<string, Action<XmlElement>>
                    {
                        {nameof(Boulder), AddBoulder},
                        {nameof(Monster), AddMonster},
                        {nameof(Crystal), AddCrystal},
                        {nameof(ForceField), AddForceFields},
                        {nameof(CrumblyWall), AddCrumblyWall}
                    };
                return result;
                }
            }
        }
    }
