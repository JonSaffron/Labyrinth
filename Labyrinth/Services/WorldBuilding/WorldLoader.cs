﻿using System;
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
        private XmlDocument _xmlDoc;
        private List<WorldArea> _worldAreas;
        
        public void LoadWorld(string levelName)
            {
            string levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Levels/", levelName);
            if (!File.Exists(levelPath))
                throw new ArgumentOutOfRangeException(levelPath);
            
            this._xmlDoc = new XmlDocument();
            this._xmlDoc.Load(levelPath);
            this._worldAreas = LoadAreas(this._xmlDoc);
            }
        
        public Tile[,] GetFloorTiles()
            {
            var result = GetFloorLayout();
            return result;
            }

        public void GetGameObjects(GameState gameState)
            {
            var x = new ProcessGameObjects(this.Width, this.Height, this._worldAreas);
            var layoutDef = (XmlElement) this._xmlDoc.SelectSingleNode("World/Layout");
            if (layoutDef == null)
                throw new InvalidOperationException("No Layout element found.");
            var layout = layoutDef.InnerText;
            var objectsDef = (XmlElement) this._xmlDoc.SelectSingleNode("World/Objects");
            if (objectsDef == null)
                throw new InvalidOperationException("No objects element found.");
            x.GetGameObjects(gameState, layout, objectsDef.SelectNodes("*"));
            }

        /// <summary>
        /// Width of World measured in tiles.
        /// </summary>
        private int Width
            {
            get 
                { 
                var worldDef = (XmlElement) this._xmlDoc.SelectSingleNode("World");
                if (worldDef == null)
                    throw new InvalidOperationException("No World element found.");
                int result = int.Parse(worldDef.GetAttribute("Width"));
                return result;
                }
            }

        /// <summary>
        /// Height of the World measured in tiles.
        /// </summary>
        private int Height
            {
            get 
                { 
                var worldDef = (XmlElement) this._xmlDoc.SelectSingleNode("World");
                if (worldDef == null)
                    throw new InvalidOperationException("No World element found.");
                int result = int.Parse(worldDef.GetAttribute("Height"));
                return result;
                }
            }

        private static List<WorldArea> LoadAreas(XmlDocument xmlDoc)
            {
            var areas = xmlDoc.SelectSingleNode("World/Areas");
            if (areas == null)
                throw new InvalidOperationException();
            var result = new List<WorldArea>();
            XmlNodeList areaList = areas.SelectNodes("Area");
            if (areaList == null)
                throw new InvalidOperationException();
            foreach (XmlElement a in areaList)
                {
                var wa = new WorldArea(a);
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
            var layout = (XmlElement) this._xmlDoc.SelectSingleNode("World/Layout");
            if (layout == null)
                throw new InvalidOperationException("No Layout element found.");
            
            var lines = layout.InnerText.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
            ValidateLayout(lines);
               
            var result = new Tile[this.Width, this.Height];
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
            int countOfLines = lines.GetLength(0);
            if (countOfLines != this.Height)
                throw new InvalidOperationException(string.Format("The world layout has {0} lines whilst the Height property equals {1}. The two should match.", countOfLines, this.Height));
            
            for (int y = 0; y < this.Height; y++)
                {
                var lineLength = lines[y].Length;
                if (lineLength != this.Width)
                    throw new InvalidOperationException(string.Format("Line {0} has {1} characters, whilst the Width property equals {2}. The two should match.", y, lineLength, this.Width));
                }
            }

        class ProcessGameObjects
            {
            readonly TileUsage[,] _tileUsage;
            readonly IEnumerable<WorldArea> _worldAreas;

            public ProcessGameObjects(int width, int height, IEnumerable<WorldArea> worldAreas)
                {
                this._tileUsage = new TileUsage[width, height];
                this._worldAreas = worldAreas;
                }

            public void GetGameObjects(GameState gameState, string layout, XmlNodeList objects)
                {
                if (gameState == null)
                    throw new ArgumentNullException("gameState");
                if (layout == null)
                    throw new ArgumentNullException("layout");
                if (objects == null)
                    throw new ArgumentNullException("objects");

                this.AddWalls(gameState, layout);

                WorldArea initialWorldArea = this.GetInitialWorldArea();
                StartState ss = initialWorldArea.StartState;
                gameState.AddPlayer(ss.PlayerPosition.ToPosition(), ss.PlayerEnergy, initialWorldArea.Id);
            
                var exceptions = new List<TileException>();
                var playerStartPositions = this._worldAreas.Where(wa => wa.StartState != null).Select(wa => new StartPositionReservation(wa.StartState.PlayerPosition));
                exceptions.AddRange(SetTileOccupation(playerStartPositions, t => t.SetOccupationByStaticItem() ));

                var movingMonsters = new List<IGameObject>();
                foreach (XmlElement definition in objects)
                    {
                    StaticItem[] newItems = {}; 

                    switch (definition.LocalName)
                        {
                        case "Boulder":
                            {
                            newItems = new StaticItem[] { GetBoulder(gameState, definition) };
                            break;
                            }

                        case "Monster":
                            {
                            var monster = GetMonster(gameState, definition);
                            if (monster.IsStill)
                                newItems = new StaticItem[] { monster };
                            else
                                movingMonsters.Add(monster);
                            break;
                            }

                        case "Crystal":
                            {
                            newItems = new StaticItem[] { GetCrystal(gameState, definition) };
                            break;
                            }

                        case "ForceField":
                            {
                            newItems = GetForceFields(gameState, definition).Cast<StaticItem>().ToArray();
                            break;
                            }

                        case "CrumblyWall":
                            {
                            newItems = new StaticItem[] { GetCrumblyWall(gameState, definition) };
                            break;
                            }

                        default:
                            throw new InvalidOperationException("Unknown object " + definition.LocalName);
                        }

                    exceptions.AddRange(SetTileOccupation(newItems, t => t.SetOccupationByStaticItem()));
                    }

                GetListOfFruit(gameState);

                exceptions.AddRange(SetTileOccupation(movingMonsters, t => t.SetOccupationByMovingMonster()));
            
                ReviewPotentiallyOccupiedTiles(exceptions);
            
                ReviewExceptionList(exceptions);
                }

            private WorldArea GetInitialWorldArea()
                {
                WorldArea result;
                try
                    {
                    result = this._worldAreas.Single(wa => wa.IsInitialArea);
                    }
                catch (InvalidOperationException)
                    {
                    throw new InvalidOperationException("There is not a single worldarea marked as the initial area.");
                    }
                return result;
                }

            private void AddWalls(GameState gameState, string layout)
                {
                var lines = layout.Trim().Replace("\r\n", "\t").Split('\t').Select(line => line.Trim()).ToArray();
               
                foreach (WorldArea wa in this._worldAreas)
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
                                gameState.AddWall(p.ToPosition(), "Tiles/" + td.TextureName);
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

            private static Monster GetMonster(GameState gameState, XmlElement mdef)
                {
                string type = mdef.GetAttribute("Type");
                var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
                Vector2 position = tilePos.ToPosition();
                int e = int.Parse(mdef.GetAttribute("Energy"));
                Monster result = gameState.CreateMonster(type, position, e);
                string initialDirection = mdef.GetAttribute("InitialDirection");
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
            
                return result;
                }

            private static Crystal GetCrystal(GameState gameState, XmlElement cdef)
                {
                var id = int.Parse(cdef.GetAttribute("Id"));
                var tilePos = new TilePos(int.Parse(cdef.GetAttribute("Left")), int.Parse(cdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var score = int.Parse(cdef.GetAttribute("Score"));
                var energy = int.Parse(cdef.GetAttribute("Energy"));
                var result = gameState.AddCrystal(position, id, score, energy);
                return result;
                }

            private static Boulder GetBoulder(GameState gameState, XmlElement bdef)
                {
                var tilePos = new TilePos(int.Parse(bdef.GetAttribute("Left")), int.Parse(bdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var result = gameState.AddBoulder(position);
                return result;
                }

            private static IEnumerable<ForceField> GetForceFields(GameState gameState, XmlElement fdef)
                {
                int crystalRequired = int.Parse(fdef.GetAttribute("CrystalRequired"));
                Rectangle r = WorldArea.GetRectangleFromDefinition(fdef);
                var result = r.PointsInside().Select(tp => gameState.AddForceField(tp.ToPosition(), crystalRequired));
                return result;
                }

            private static CrumblyWall GetCrumblyWall(GameState gameState, XmlElement wdef)
                {
                var tilePos = new TilePos(int.Parse(wdef.GetAttribute("Left")), int.Parse(wdef.GetAttribute("Top")));
                var position = tilePos.ToPosition();
                var energy = int.Parse(wdef.GetAttribute("Energy"));
                var textureName = wdef.GetAttribute("Texture");
                var result = gameState.AddCrumblyWall(position, "Tiles/" + textureName, energy);
                return result;
                }

            private void GetListOfFruit(GameState gameState)
                {
                var rnd = GlobalServices.Randomess;
                var result = new List<Fruit>();
                foreach (WorldArea wa in this._worldAreas)
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
                            var f = gameState.AddFruit(position, fd.FruitType);
                            result.Add(f);
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
