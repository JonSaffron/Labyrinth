using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public void GetGameObjects(GameState gameState)
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
                        throw new InvalidOperationException(string.Format("The area {0} intersects with another area {1} (this is a problem because there are multiple tile definitions).", wa.Area, intersectingArea.Area));
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
                
                string defaultFloorName = wa.TileDefinitions.Values.Single(td => td.TileTypeByMap == TileTypeByMap.Floor).TextureName;
                foreach (TilePos p in wa.Area.PointsInside())
                    {
                    char symbol = lines[p.Y][p.X];
                    TileDefinition td;
                    if (!wa.TileDefinitions.TryGetValue(symbol, out td))
                        {
                        string text = string.Format("Don't know what symbol {0} indicates in world area {1}", symbol, wa.Id);
                        throw new InvalidOperationException(text);
                        }
                    Texture2D floor;
                    switch (td.TileTypeByMap)
                        {
                        case TileTypeByMap.Wall:
                        case TileTypeByMap.PotentiallyOccupied:
                            {
                            floor = spriteLibrary.GetSprite("Tiles/" + defaultFloorName);
                            break;
                            }
                        case TileTypeByMap.Floor:
                            {
                            floor = spriteLibrary.GetSprite("Tiles/" + td.TextureName);
                            break;
                            }
                        default:
                            throw new InvalidOperationException();
                        }

                    if (!wa.Id.HasValue)
                        throw new InvalidOperationException("Area with tile definitions needs an id.");
                    result[p.X, p.Y] = new Tile(floor, wa.Id.Value);
                    }
                }
            return result;
            }

        private void ValidateLayout(string[] lines)
            {
            TilePos size = this.Size;
            int countOfLines = lines.GetLength(0);
            if (countOfLines != size.Y)
                throw new InvalidOperationException(string.Format("The world layout has {0} lines whilst the Height property equals {1}. The two should match.", countOfLines, size.Y));
            
            for (int y = 0; y < size.Y; y++)
                {
                var lineLength = lines[y].Length;
                if (lineLength != size.X)
                    throw new InvalidOperationException(string.Format("Line {0} has {1} characters, whilst the Width property equals {2}. The two should match.", y, lineLength, size.X));
                }
            }

        internal class ProcessGameObjects
            {
            private readonly WorldLoader _wl;
            private readonly GameState _gameState;
            private readonly TileUsage[,] _tileUsage;
            private readonly string _layout;

            public ProcessGameObjects(WorldLoader wl, GameState gameState)
                {
                this._wl = wl;
                this._gameState = gameState;
                this._tileUsage = new TileUsage[wl.Size.X, wl.Size.Y];

                var layoutDef = (XmlElement) wl._xmlRoot.SelectSingleNode("ns:Layout", wl._xnm);
                if (layoutDef == null)
                    throw new InvalidOperationException("No Layout element found.");
                this._layout = layoutDef.InnerText;
                }

            public void AddGameObjects()
                {
                var objects = this._wl._xmlRoot.SelectNodes(@"ns:Objects/ns:*", this._wl._xnm);
                var objectList = objects != null ? objects.Cast<XmlElement>() : Enumerable.Empty<XmlElement>();

                this.AddWalls();

                WorldArea initialWorldArea = this.GetInitialWorldArea();
                PlayerStartState pss = initialWorldArea.PlayerStartState;
                if (pss == null)
                    throw new InvalidOperationException("Initial world area should have a player start state.");
                this._gameState.AddPlayer(pss.Position.ToPosition(), pss.Energy, initialWorldArea.Id);
            
                var exceptions = new List<TileException>();
                var playerStartPositions = this._wl._worldAreas.Where(wa => wa.PlayerStartState != null).Select(wa => new StartPositionReservation(wa.PlayerStartState.Position));
                exceptions.AddRange(SetTileOccupation(playerStartPositions, t => t.SetOccupationByStaticItem() ));

                var movingMonsters = new List<IGameObject>();
                foreach (XmlElement definition in objectList)
                    {
                    StaticItem[] newItems = {}; 

                    switch (definition.LocalName)
                        {
                        case "Boulder":
                            {
                            newItems = new StaticItem[] { GetBoulder(definition) };
                            break;
                            }

                        case "Monster":
                            {
                            var monster = GetMonster(definition);
                            if (monster.IsStatic)
                                newItems = new StaticItem[] { monster };
                            else
                                movingMonsters.Add(monster);
                            break;
                            }

                        case "Crystal":
                            {
                            newItems = new StaticItem[] { GetCrystal(definition) };
                            break;
                            }

                        case "ForceField":
                            {
                            newItems = GetForceFields(definition).Cast<StaticItem>().ToArray();
                            break;
                            }

                        case "CrumblyWall":
                            {
                            newItems = new StaticItem[] { GetCrumblyWall(definition) };
                            break;
                            }

                        default:
                            throw new InvalidOperationException("Unknown object " + definition.LocalName);
                        }

                    exceptions.AddRange(SetTileOccupation(newItems, t => t.SetOccupationByStaticItem()));
                    }

                var monsters = GetMonstersFromRandomDistribution().ToList();
                foreach (Monster monster in monsters)
                    {
                    if (monster.IsStatic)
                        exceptions.AddRange(SetTileOccupation(new [] { monster }, t => t.SetOccupationByRandomMonsterDistribution()));
                    }

                AddFruit();

                exceptions.AddRange(SetTileOccupation(movingMonsters, t => t.SetOccupationByMovingMonster()));
                exceptions.AddRange(SetTileOccupation(monsters, t => t.SetOccupationByMovingRandomlyDistributedMonster()));
            
                ReviewPotentiallyOccupiedTiles(exceptions);
            
                ReviewExceptionList(exceptions);
                }

            private WorldArea GetInitialWorldArea()
                {
                WorldArea result;
                try
                    {
                    result = this._wl._worldAreas.Single(wa => wa.IsInitialArea);
                    }
                catch (InvalidOperationException)
                    {
                    throw new InvalidOperationException("There is not a single worldarea marked as the initial area.");
                    }
                return result;
                }

            private void AddWalls()
                {
                var lines = _layout.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
                
                foreach (WorldArea wa in this._wl._worldAreas)
                    {
                    var tileDefs = wa.TileDefinitions;
                    if (tileDefs == null || tileDefs.Count == 0)
                        continue;
                
                    foreach (TilePos p in wa.Area.PointsInside())
                        {
                        char symbol = lines[p.Y][p.X];
                        TileDefinition td;
                        if (!tileDefs.TryGetValue(symbol, out td))
                            {
                            string text = string.Format("Don't know what symbol {0} indicates in world area {1}", symbol, wa.Id.HasValue ? (object) wa.Id.Value : "(no number)");
                            throw new InvalidOperationException(text);
                            }
                        switch (td.TileTypeByMap)
                            {
                            case TileTypeByMap.Wall:
                                {
                                this._tileUsage[p.X, p.Y] = new TileUsage(TileTypeByMap.Wall);
                                this._gameState.AddWall(p.ToPosition(), "Tiles/" + td.TextureName);
                                break;
                                }
                            case TileTypeByMap.Floor:
                                {
                                this._tileUsage[p.X, p.Y] = new TileUsage(TileTypeByMap.Floor);
                                break;
                                }
                            case TileTypeByMap.PotentiallyOccupied:
                                {
                                this._tileUsage[p.X, p.Y] = new TileUsage(symbol);  
                                break;
                                }
                            default:
                                throw new InvalidOperationException();
                            }
                        }
                    }
                }

            private IEnumerable<TileException> SetTileOccupation(IEnumerable<IGameObject> gameObjects, Action<TileUsage> action)
                {
                var result = new List<TileException>();
                foreach (var item in gameObjects)
                    {
                    TilePos tp = item.TilePosition;

                    try
                        {
                        action(this._tileUsage[tp.X, tp.Y]);
                        }
                    catch (TileException te)
                        {
                        var nte = new TileException(te, string.Format("{0} at tile {1},{2} for {3}", item.GetType().Name, tp.X, tp.Y, action.Method.Name));
                        result.Add(nte);
                        }
                    }
                return result;
                }

            private void ReviewPotentiallyOccupiedTiles(ICollection<TileException> exceptions)
                {
                var cy = this._tileUsage.GetLength(1);
                var cx = this._tileUsage.GetLength(0);
                for (int y = 0; y < cy; y++)
                    {
                    for (int x = 0; x < cx; x++)
                        {
                        TileUsage t = this._tileUsage[x, y];
                        try
                            {
                            t.CheckIfIncorrectlyPotentiallyOccupied();
                            }
                        catch (TileException te)
                            {
                            var nte = new TileException(te, string.Format("Tile at {0},{1}", x, y));
                            exceptions.Add(nte);
                            }
                        }
                    }
                }

            private static void ReviewExceptionList(List<TileException> exceptions)
                {
                if (!exceptions.Any())
                    return;

                var message = new StringBuilder();
                var errors = exceptions.Where(te=>te.Type == TileExceptionType.Error).ToList();
                var warnings = exceptions.Where(te=>te.Type == TileExceptionType.Warning).ToList();
                foreach (TileException te in errors)
                    {
                    if (message.Length != 0)
                        message.AppendLine();
                    message.AppendFormat("{0} - {1}", te.Type, te.Message);
                    }
                if (warnings.Any())
                    {
                    if (message.Length != 0)
                        message.AppendLine();
                    foreach (TileException te in warnings)
                        {
                        if (message.Length != 0)
                            message.AppendLine();
                        message.AppendFormat("{0} - {1}", te.Type, te.Message);
                        }
                    }
                if (errors.Any())
                    throw new InvalidOperationException(message.ToString());

                Trace.WriteLine(message);
                var dr = MessageBox.Show(message.ToString(), "Warnings", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    throw new InvalidOperationException(message.ToString());
                }

            private Monster GetMonster(XmlElement mdef)
                {
                MonsterDef md = MonsterDef.FromXml(mdef);
                var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
                md.Position = tilePos.ToPosition();

                var result = this._gameState.CreateMonster(md);
                return result;
                }


            private Crystal GetCrystal(XmlElement cdef)
                {
                var id = int.Parse(cdef.GetAttribute("Id"));
                var tilePos = new TilePos(int.Parse(cdef.GetAttribute("Left")), int.Parse(cdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var score = int.Parse(cdef.GetAttribute("Score"));
                var energy = int.Parse(cdef.GetAttribute("Energy"));
                var result = this._gameState.AddCrystal(position, id, score, energy);
                return result;
                }

            private Boulder GetBoulder(XmlElement bdef)
                {
                var tilePos = new TilePos(int.Parse(bdef.GetAttribute("Left")), int.Parse(bdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var result = this._gameState.AddBoulder(position);
                return result;
                }

            private IEnumerable<ForceField> GetForceFields(XmlElement fdef)
                {
                int crystalRequired = int.Parse(fdef.GetAttribute("CrystalRequired"));
                Rectangle r = WorldArea.GetRectangleFromDefinition(fdef);
                var result = r.PointsInside().Select(tp => this._gameState.AddForceField(tp.ToPosition(), crystalRequired));
                return result;
                }

            private CrumblyWall GetCrumblyWall(XmlElement wdef)
                {
                var tilePos = new TilePos(int.Parse(wdef.GetAttribute("Left")), int.Parse(wdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var energy = int.Parse(wdef.GetAttribute("Energy"));
                var textureName = wdef.GetAttribute("Texture");
                var result = this._gameState.AddCrumblyWall(position, "Tiles/" + textureName, energy);
                return result;
                }

            private IEnumerable<Monster> GetMonstersFromRandomDistribution()
                {
                var rnd = GlobalServices.Randomess;
                var dists = 
                    this._wl._worldAreas.Where(wa => wa.RandomMonsterDistribution != null)
                        .Select(wa => new { wa.Area, Dist = wa.RandomMonsterDistribution});
                var result = new List<Monster>();
                foreach (var item in dists)
                    {
                    for (int i = 0; i < item.Dist.CountOfMonsters; i++)
                        {
                        var monsterIndex = rnd.DiceRoll(item.Dist.DiceRoll);
                        var monsterDef = item.Dist.Templates[monsterIndex];

                        TilePos tp = GetFreeTile(item.Area);
                        monsterDef.Position = tp.ToPosition();
                        result.Add(this._gameState.CreateMonster(monsterDef));
                        i++;
                        }
                    }
                return result;
                }

            private void AddFruit()
                {
                var fruitDefinitions =
                    this._wl._worldAreas.Where(wa => wa.FruitDefinitions != null)
                        .SelectMany(wa => wa.FruitDefinitions.Values, (wa, fd) => new {wa.Area, FruitDefinition = fd});
                foreach (var item in fruitDefinitions)
                    {
                    for (int i = 0; i < item.FruitDefinition.FruitQuantity;)
                        {
                        TilePos tp = GetFreeTile(item.Area);
                        this._tileUsage[tp.X, tp.Y].SetOccupationByFruit();
                        Vector2 position = tp.ToPosition();
                        this._gameState.AddFruit(position, item.FruitDefinition.FruitType, item.FruitDefinition.Energy);
                        i++;
                        }
                    }
                }

            private TilePos GetFreeTile(Rectangle area)
                {
                var rnd = GlobalServices.Randomess;
                while (true)
                    {
                    int x = area.X + rnd.Next(area.Width);
                    int y = area.Y + rnd.Next(area.Height);
                    TileUsage t = this._tileUsage[x, y];
                    if (t.IsFree)
                        {
                        var result = new TilePos(x, y);
                        return result;
                        }
                    }
                }

            private class StartPositionReservation : IGameObject
                {
                private readonly TilePos _tilePosition;

                public StartPositionReservation(TilePos tilePosition)
                    {
                    this._tilePosition = tilePosition;
                    }

                public Vector2 Position
                    {
                    get { throw new InvalidOperationException(); }
                    }

                public TilePos TilePosition
                    {
                    get { return this._tilePosition; }
                    }

                public bool IsExtant
                    {
                    get { throw new InvalidOperationException(); }
                    }
                }
            }
        }
    }
