using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private Tile[,] _tiles;
        
        private static readonly Random Rnd = new Random();
        
        public void LoadWorld(string levelName)
            {
            string levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/Levels/", levelName);
            if (!File.Exists(levelPath))
                throw new ArgumentOutOfRangeException(levelPath);
            
            this._xmlDoc = new XmlDocument();
            this._xmlDoc.Load(levelPath);
            this._worldAreas = LoadAreas();
            }
        
        /// <summary>
        /// Width of World measured in tiles.
        /// </summary>
        public int Width
            {
            get 
                { 
                var result = this._tiles.GetLength(0);
                return result;
                }
            }

        /// <summary>
        /// Height of the World measured in tiles.
        /// </summary>
        public int Height
            {
            get 
                { 
                var result = this._tiles.GetLength(1);
                return result;
                }
            }
        
        public Tile this[TilePos tp]
            {
            get
                {
                return this._tiles[tp.X, tp.Y];
                }
            }

        private List<WorldArea> LoadAreas()
            {
            var areas = this._xmlDoc.SelectSingleNode("World/Areas");
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

        private int GetStartingWorldAreaId()
            {
            WorldArea startingWorldArea;
            try
                {
                startingWorldArea = this._worldAreas.Single(wa => wa.IsInitialArea);
                }
            catch (InvalidOperationException)
                {
                throw new InvalidOperationException("There is not a single worldarea marked as the initial area.");
                }
            var result = startingWorldArea.Id;
            return result;
            }

        public int GetMaximumWorldAreaId()
            {
            var worldAreasWithIds = this._worldAreas.Where(wa => wa.HasId).ToList();
            if (!worldAreasWithIds.Any())
                throw new InvalidOperationException();
            var result = worldAreasWithIds.Max(wa => wa.Id);
            return result;
            }

        public StartState GetStartStateForWorldAreaId(int id)
            {
            var worldArea = this._worldAreas.SingleOrDefault(wa => wa.HasId && wa.Id == id);
            if (worldArea == null)
                return null;
            var result = worldArea.StartState;
            return result;
            }

        public int GetWorldAreaIdForTilePos(TilePos tp)
            {
            var areaContainingPoint =
                (from WorldArea wa in this._worldAreas
                where wa.HasId && wa.StartState != null && wa.Area.Contains(tp.X, tp.Y)
                select wa).Single();
            var result = areaContainingPoint.Id;
            return result;
            }
        
        public IEnumerable<StaticItem> GetGameObjects(World world)
            {
            var exceptions = new List<TileException>();
            Tile[,] tiles;

            var result = new List<StaticItem>();
            var walls = GetLayout(world, out tiles);
            result.AddRange(walls);

            int initialWorldId = GetStartingWorldAreaId();
            StartState ss = GetStartStateForWorldAreaId(initialWorldId);

            var player = new Player(world, ss.PlayerPosition.ToPosition(), ss.PlayerEnergy);
            result.Add(player);
            var playerStartPositions = this._worldAreas.Where(wa => wa.StartState != null).Select(wa => new Grave(world, wa.StartState.PlayerPosition.ToPosition()));
            exceptions.AddRange(SetTileOccupation(ref tiles, playerStartPositions, false));

            var objects = this._xmlDoc.SelectNodes("World/Objects/*");
            if (objects == null)
                throw new InvalidOperationException("No Objects node in world definition.");
            foreach (XmlElement definition in objects)
                {
                StaticItem[] newItems; 

                switch (definition.LocalName)
                    {
                    case "Boulder":
                        {
                        newItems = new StaticItem[] { GetBoulder(world, definition) };
                        break;
                        }

                    case "Monster":
                        {
                        newItems = new StaticItem[] { GetMonster(world, definition) };
                        break;
                        }

                    case "Crystal":
                        {
                        newItems = new StaticItem[] { GetCrystal(world, definition) };
                        break;
                        }

                    case "ForceField":
                        {
                        newItems = GetForceFields(world, definition).Cast<StaticItem>().ToArray();
                        break;
                        }

                    case "CrumblyWall":
                        {
                        newItems = new StaticItem[] { GetCrumblyWall(world, definition) };
                        break;
                        }

                    default:
                        throw new InvalidOperationException("Unknown object " + definition.LocalName);
                    }

                result.AddRange(newItems);
                exceptions.AddRange(SetTileOccupation(ref tiles, newItems.Where(item => !(item is Monster) || ((Monster) item).IsStill), false));
                }

            var fruit = GetListOfFruit(world, ref tiles);
            result.AddRange(fruit);

            exceptions.AddRange(SetTileOccupation(ref tiles, result.OfType<Monster>().Where(m => !m.IsStill), true));
            
            ReviewPotentiallyOccupiedTiles(ref tiles, exceptions);
            
            if (exceptions.Count != 0)
                {
                string message = string.Empty;
                var errors = exceptions.Where(te=>te.Type == TileExceptionType.Error).ToList();
                var warnings = exceptions.Where(te=>te.Type == TileExceptionType.Warning).ToList();
                foreach (TileException te in errors)
                    {
                    if (message.Length != 0)
                        message += "\r\n";
                    message += string.Format("{0} - {1}", te.Type, te.Message);
                    }
                if (warnings.Any())
                    {
                    if (message.Length != 0)
                        message += "\r\n";
                    foreach (TileException te in warnings)
                        {
                        if (message.Length != 0)
                            message += "\r\n";
                        message += string.Format("{0} - {1}", te.Type, te.Message);
                        }
                    }
                if (errors.Any())
                    throw new InvalidOperationException(message);
                Trace.WriteLine(message);
                var dr = MessageBox.Show(message, "Warnings", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                    throw new InvalidOperationException(message);
                }

            this._tiles = tiles;
            return result;
            }

        private static Monster GetMonster(World world, XmlElement mdef)
            {
            string type = mdef.GetAttribute("Type");
            var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
            Vector2 position = tilePos.ToPosition();
            int e = int.Parse(mdef.GetAttribute("Energy"));
            Monster result = Monster.Create(type, world, position, e);
            string direction = mdef.GetAttribute("Direction");
            if (!string.IsNullOrEmpty(direction))
                result.Direction = (Direction)Enum.Parse(typeof(Direction), direction);
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

        private static Crystal GetCrystal(World world, XmlElement cdef)
            {
            var id = int.Parse(cdef.GetAttribute("Id"));
            var tilePos = new TilePos(int.Parse(cdef.GetAttribute("Left")), int.Parse(cdef.GetAttribute("Top")));
            var position = tilePos.ToPosition();
            var score = int.Parse(cdef.GetAttribute("Score"));
            var energy = int.Parse(cdef.GetAttribute("Energy"));
            var result = new Crystal(world, position, id, score, energy);
            return result;
            }

        private static Boulder GetBoulder(World world, XmlElement bdef)
            {
            var tilePos = new TilePos(int.Parse(bdef.GetAttribute("Left")), int.Parse(bdef.GetAttribute("Top")));
            var position = tilePos.ToPosition();
            var result = new Boulder(world, position);
            return result;
            }

        private static IEnumerable<ForceField> GetForceFields(World world, XmlElement fdef)
            {
            int crystalRequired = int.Parse(fdef.GetAttribute("CrystalRequired"));
            Rectangle r = WorldArea.GetRectangleFromDefinition(fdef);
            var result = r.PointsInside().Select(tp => new ForceField(world, tp.ToPosition(), crystalRequired));
            return result;
            }

        private static CrumblyWall GetCrumblyWall(World world, XmlElement wdef)
            {
            var tilePos = new TilePos(int.Parse(wdef.GetAttribute("Left")), int.Parse(wdef.GetAttribute("Top")));
            var position = tilePos.ToPosition();
            var energy = int.Parse(wdef.GetAttribute("Energy"));
            var textureName = wdef.GetAttribute("Texture");
            var result = new CrumblyWall(world, position, "Tiles/" + textureName, energy);
            return result;
            }

        private IEnumerable<Fruit> GetListOfFruit(World world, ref Tile[,] tiles)
            {
            var result = new List<Fruit>();
            foreach (WorldArea wa in this._worldAreas)
                {
                foreach (FruitDefinition fd in wa.FruitDefinitions.Values)
                    {
                    for (int i = 0; i < fd.FruitQuantity; )
                        {
                        var tilePos = new TilePos(wa.Area.X + Rnd.Next(wa.Area.Width), wa.Area.Y + Rnd.Next(wa.Area.Height));
                        Tile t = tiles[tilePos.X, tilePos.Y];
                        if (!t.IsFree)
                            continue;
                        tiles[tilePos.X, tilePos.Y].SetOccupationByFruit();
                        Vector2 position = tilePos.ToPosition();
                        var f = new Fruit(world, position, fd.FruitType);
                        result.Add(f);
                        i++;
                        }
                    }
                }
            return result;
            }

        private IEnumerable<Wall> GetLayout(World world, out Tile[,] tiles)
            {
            var worldDef = (XmlElement)this._xmlDoc.SelectSingleNode("World");
            if (worldDef == null)
                throw new InvalidOperationException("No World element found.");
            int width = int.Parse(worldDef.GetAttribute("Width"));
            int height = int.Parse(worldDef.GetAttribute("Height"));
            var layout = (XmlElement) this._xmlDoc.SelectSingleNode("World/Layout");
            if (layout == null)
                throw new InvalidOperationException("No Layout element found.");
            tiles = new Tile[width, height];            
            
            var lines = layout.InnerText.Trim().Replace("\r\n", "\t").Split('\t');
            if (lines.GetLength(0) != height)
                throw new InvalidOperationException();
            
            for (int y = 0; y < height; y++)
                {
                lines[y] = lines[y].Trim();
                if (lines[y].Length != width)
                    throw new InvalidOperationException();
                }
               
            var result = new List<Wall>();
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
                            floor = world.LoadTexture("Tiles/" + defaultFloorName);
                            tiles[p.X, p.Y] = new Tile(floor, TileTypeByMap.Wall);
                            var wall = new Wall(world, p.ToPosition(), "Tiles/" + td.TextureName);
                            result.Add(wall);
                            break;
                        case TileTypeByMap.Floor:
                            floor = world.LoadTexture("Tiles/" + td.TextureName);
                            tiles[p.X, p.Y] = new Tile(floor, TileTypeByMap.Floor);
                            break;
                        case TileTypeByMap.PotentiallyOccupied:
                            floor = world.LoadTexture("Tiles/" + defaultFloorName);
                            tiles[p.X, p.Y] = new Tile(floor, symbol);  
                            break;
                        default:
                            throw new InvalidOperationException();
                        }
                    }
                }
            
            return result;
            }

        private static IEnumerable<TileException> SetTileOccupation(ref Tile[,] tiles, IEnumerable<StaticItem> gameObjects, bool isMoving)
            {
            Action<Tile> action;
            if (isMoving)
                action = t => t.SetOccupationByMovingMonster();
            else
                action = t => t.SetOccupationByStaticItem();

            var result = new List<TileException>();
            foreach (var item in gameObjects)
                {
                TilePos tp = item.TilePosition;

                try
                    {
                    action(tiles[tp.X, tp.Y]);
                    }
                catch (TileException te)
                    {
                    var nte = new TileException(te, string.Format("{0} at tile {1},{2} for {3} occupation", item.GetType().Name, tp.X, tp.Y, isMoving ? "moving" : "static"));
                    result.Add(nte);
                    }
                }
            return result;
            }

        private static void ReviewPotentiallyOccupiedTiles(ref Tile[,] tiles, ICollection<TileException> exceptions)
            {
            var cy = tiles.GetLength(1);
            var cx = tiles.GetLength(0);
            for (int y = 0; y < cy; y++)
                {
                for (int x = 0; x < cx; x++)
                    {
                    Tile t = tiles[x, y];
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
        }
    }
