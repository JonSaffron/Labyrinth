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
    class WorldLoader : IWorldLoader
        {
        private XmlElement _xmlRoot;
        private XmlNamespaceManager _xnm;
        private List<WorldArea> _worldAreas;
        
        public void LoadWorld(string levelName)
            {
            string levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Levels/", levelName);
            if (!File.Exists(levelPath))
                throw new ArgumentOutOfRangeException(levelPath);
            
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(levelPath);
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

        public void GetGameObjects(GameState gameState)
            {
            var x = new ProcessGameObjects(this, gameState);
            x.GetGameObjects();
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
                if (wa.TileDefinitions.Count != 0)
                    {
                    WorldArea intersectingArea =
                        (from WorldArea r in result
                        where r.Area.Intersects(wa.Area) && r.TileDefinitions.Count != 0
                        select r).FirstOrDefault();
                    if (intersectingArea != null)
                        throw new InvalidOperationException(string.Format("The area {0} intersects with another area {1} (this is a problem because there are multiple tile definitions).", wa.Area, intersectingArea.Area));
                    }
                
                result.Add(wa);
                }
            return result;
            }

        public Dictionary<int, StartState> GetStartStates()
            {
            var result = this._worldAreas.Where(item => item.HasId).ToDictionary(key => key.Id, value => value.StartState);
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
                if (wa.TileDefinitions.Count == 0)
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
                    result[p.X, p.Y] = new Tile(floor, wa.Id);
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

        private class ProcessGameObjects
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

            public void GetGameObjects()
                {
                var objects = this._wl._xmlRoot.SelectNodes(@"ns:Objects/ns:*", this._wl._xnm);
                var objectList = objects != null ? objects.Cast<XmlElement>() : Enumerable.Empty<XmlElement>();

                this.AddWalls();

                WorldArea initialWorldArea = this.GetInitialWorldArea();
                StartState ss = initialWorldArea.StartState;
                this._gameState.AddPlayer(ss.PlayerPosition.ToPosition(), ss.PlayerEnergy, initialWorldArea.Id);
            
                var exceptions = new List<TileException>();
                var playerStartPositions = this._wl._worldAreas.Where(wa => wa.StartState != null).Select(wa => new StartPositionReservation(wa.StartState.PlayerPosition));
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
                            if (monster.IsStill)
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

                GetListOfFruit();

                exceptions.AddRange(SetTileOccupation(movingMonsters, t => t.SetOccupationByMovingMonster()));
            
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
                    if (wa.TileDefinitions.Count == 0)
                        continue;
                
                    foreach (TilePos p in wa.Area.PointsInside())
                        {
                        char symbol = lines[p.Y][p.X];
                        TileDefinition td;
                        if (!wa.TileDefinitions.TryGetValue(symbol, out td))
                            {
                            string text = string.Format("Don't know what symbol {0} indicates in world area {1}", symbol, wa.Id);
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
                string type = mdef.GetAttribute("Type");
                var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
                Vector2 position = tilePos.ToPosition();
                int e = int.Parse(mdef.GetAttribute("Energy"));
                Monster result = this._gameState.CreateMonster(type, position, e);
                string initialDirection = mdef.GetAttribute("Direction");
                if (!string.IsNullOrEmpty(initialDirection))
                    result.InitialDirection = (Direction)Enum.Parse(typeof(Direction), initialDirection);
                string mobility = mdef.GetAttribute("Mobility");
                if (!String.IsNullOrEmpty(mobility))
                    result.Mobility = (MonsterMobility)Enum.Parse(typeof(MonsterMobility), mobility);
                string changeRooms = mdef.GetAttribute("ChangeRooms");
                if (!string.IsNullOrEmpty(changeRooms))
                    result.ChangeRooms = (ChangeRooms)Enum.Parse(typeof(ChangeRooms), changeRooms);
                string isEggAttribute = mdef.GetAttribute("IsEgg");
                string timeBeforeHatchingAttribute = mdef.GetAttribute("TimeBeforeHatching");
                if (!string.IsNullOrEmpty(isEggAttribute) && !string.IsNullOrEmpty(timeBeforeHatchingAttribute))
                    {
                    bool isEgg = Boolean.Parse(isEggAttribute);
                    int timeBeforeHatching = int.Parse(timeBeforeHatchingAttribute);
                    if (isEgg)
                        {
                        result.SetDelayBeforeHatching(timeBeforeHatching | 1);
                        }
                    }
                string laysMushrooms = mdef.GetAttribute("LaysMushrooms");
                if (!string.IsNullOrEmpty(laysMushrooms))
                    {
                    result.LaysMushrooms = Boolean.Parse(laysMushrooms);
                    }
                string laysEggs = mdef.GetAttribute("LaysEggs");
                if (!string.IsNullOrEmpty(laysEggs))
                    {
                    result.LaysEggs = Boolean.Parse(laysEggs);
                    }
                string splitsOnHit = mdef.GetAttribute("SplitsOnHit");
                if (!string.IsNullOrEmpty(splitsOnHit))
                    {
                    result.SplitsOnHit = Boolean.Parse(splitsOnHit);
                    }
            
                string monsterShootBehaviourAttribute = mdef.GetAttribute("MonsterShootBehaviour");
                if (!string.IsNullOrEmpty(monsterShootBehaviourAttribute))
                    {
                    MonsterShootBehaviour monsterShootBehaviour = (MonsterShootBehaviour) Enum.Parse(typeof(MonsterShootBehaviour), monsterShootBehaviourAttribute);
                    result.MonsterShootBehaviour = monsterShootBehaviour;
                    }
                
                string shotsBounceOffAttribute = mdef.GetAttribute("ShotsBounceOff");
                if (!string.IsNullOrEmpty(shotsBounceOffAttribute))
                    {
                    bool shotsBounceOff = bool.Parse(shotsBounceOffAttribute);
                    result.ShotsBounceOff = shotsBounceOff;
                    }

                string isActiveAttribute = mdef.GetAttribute("IsActive");
                if (!string.IsNullOrEmpty(isActiveAttribute))
                    {
                    bool isActive = bool.Parse(isActiveAttribute);
                    result.IsActive = isActive;
                    }
                
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

            private void GetListOfFruit()
                {
                var rnd = GlobalServices.Randomess;
                foreach (WorldArea wa in this._wl._worldAreas)
                    {
                    foreach (FruitDefinition fd in wa.FruitDefinitions.Values)
                        {
                        for (int i = 0; i < fd.FruitQuantity; )
                            {
                            var tilePos = new TilePos(wa.Area.X + rnd.Next(wa.Area.Width), wa.Area.Y + rnd.Next(wa.Area.Height));
                            TileUsage t = this._tileUsage[tilePos.X, tilePos.Y];
                            if (!t.IsFree)
                                continue;
                            this._tileUsage[tilePos.X, tilePos.Y].SetOccupationByFruit();
                            Vector2 position = tilePos.ToPosition();
                            this._gameState.AddFruit(position, fd.FruitType);
                            i++;
                            }
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
                    get { throw new NotImplementedException(); }
                    }

                public TilePos TilePosition
                    {
                    get { return this._tilePosition; }
                    }

                public bool IsExtant
                    {
                    get { throw new NotImplementedException(); }
                    }
                }
            }
        }
    }
