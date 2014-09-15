using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class WorldLoader
        {
        private readonly XmlDocument _xmlDoc = new XmlDocument();
        private readonly ContentManager _content;
        private readonly List<WorldArea> _worldAreas;
        private readonly Dictionary<string, Texture2D> _textures;
        
        private static readonly Random Rnd = new Random();
        
        public WorldLoader(string file, ContentManager content)
            {
            this._xmlDoc.Load(file);
            this._content = content;
            this._textures = new Dictionary<string, Texture2D>();
            
            this._worldAreas = LoadAreas();
            }
        
        private Texture2D GetTexture(string name)
            {
            Texture2D result;
            if (this._textures.TryGetValue(name, out result))
                return result;
            result = this._content.Load<Texture2D>("Tiles/" + name);
            this._textures.Add(name, result);
            return result;
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
                        throw new InvalidOperationException(string.Format("The area {0} intersects with another area {1} (this is a problem because their are multiple tile definitions).", wa.Area, intersectingArea.Area));
                    }
                
                result.Add(wa);
                }
            return result;
            }
        
        public StartState StartStateForWorld
            {
            get
                {
                StartState[] result =
                    (from WorldArea wa in this._worldAreas
                    where wa.IsInitialArea
                    select wa.StartState).ToArray();
                switch (result.GetLength(0))
                    {
                    case 0:
                        throw new InvalidOperationException("No world area is marked as the starting point.");
                    case 1:
                        return result[0];
                    }
                throw new InvalidOperationException("Multiple world areas are marked as the starting point.");
                }
            }
        
        public int? GetMaximumId()
            {
            var worldAreasWithIds = this._worldAreas.Where(wa => wa.HasId).ToList();
            if (!worldAreasWithIds.Any())
                return null;
            var result = worldAreasWithIds.Max(wa => wa.Id);
            return result;
            }

        public StartState GetStartStateForWorld(int id)
            {
            var worldArea = this._worldAreas.SingleOrDefault(wa => wa.HasId && wa.Id == id);
            if (worldArea == null)
                return null;
            var result = worldArea.StartState;
            return result;
            }

        public StartState StartStateAfterDeath(TilePos tp)
            {
            return GetWorldAreaForTilePos(tp).StartState;
            }
        
        public WorldArea GetWorldAreaForTilePos(TilePos tp)
            {
            var areaContainingPoint =
                (from WorldArea wa in this._worldAreas
                where wa.StartState != null && wa.Area.Contains(tp.X, tp.Y)
                select wa).Single();
            return areaContainingPoint;
            }
        
        public LinkedList<Monster.Monster> GetListOfMonsters(World world)
            {
            var result = new LinkedList<Monster.Monster>();
            var monsters = this._xmlDoc.SelectNodes("World/Monsters/Monster");
            if (monsters == null)
                throw new InvalidOperationException();
            foreach (XmlElement mdef in monsters)
                {
                string type = mdef.GetAttribute("Type");
                var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
                Vector2 position = tilePos.ToPosition();
                if (!world.CanTileBeOccupied(tilePos, false))
                    throw new InvalidOperationException(string.Format("Position ({0}, {1}) for monster {2} is an impassable tile.", tilePos.X, tilePos.Y, type));
                int e = int.Parse(mdef.GetAttribute("Energy"));
                Monster.Monster m = Monster.Monster.Create(type, world, position, e);
                string direction = mdef.GetAttribute("Direction");
                if (!string.IsNullOrEmpty(direction))
                    m.Direction = (Direction)Enum.Parse(typeof(Direction), direction);
                string mobility = mdef.GetAttribute("Mobility");
                if (!String.IsNullOrEmpty(mobility))
                    m.Mobility = (MonsterMobility)Enum.Parse(typeof(MonsterMobility), mobility);
                string changeRooms = mdef.GetAttribute("ChangeRooms");
                if (!string.IsNullOrEmpty(changeRooms))
                    m.ChangeRooms = (ChangeRooms)Enum.Parse(typeof(ChangeRooms), changeRooms);
                string isEggAttribute = mdef.GetAttribute("IsEgg");
                string timeBeforeHatchingAttribute = mdef.GetAttribute("TimeBeforeHatching");
                if (!string.IsNullOrEmpty(isEggAttribute) && !string.IsNullOrEmpty(timeBeforeHatchingAttribute))
                    {
                    bool isEgg = Boolean.Parse(isEggAttribute);
                    int timeBeforeHatching = int.Parse(timeBeforeHatchingAttribute);
                    if (isEgg)
                        {
                        m.DelayBeforeHatching = timeBeforeHatching | 1;
                        }
                    }
                string laysMushrooms = mdef.GetAttribute("LaysMushrooms");
                if (!string.IsNullOrEmpty(laysMushrooms))
                    {
                    m.LaysMushrooms = Boolean.Parse(laysMushrooms);
                    }
                string laysEggs = mdef.GetAttribute("LaysEggs");
                if (!string.IsNullOrEmpty(laysEggs))
                    {
                    m.LaysEggs = Boolean.Parse(laysEggs);
                    }
                string splitsOnHit = mdef.GetAttribute("SplitsOnHit");
                if (!string.IsNullOrEmpty(splitsOnHit))
                    {
                    m.SplitsOnHit = Boolean.Parse(splitsOnHit);
                    }
                result.AddLast(m);
                }
            
            return result;
            }
        
        public IEnumerable<Crystal> GetListOfCrystals(World world)
            {
            var crystals = this._xmlDoc.SelectNodes("World/Crystals/Item");
            if (crystals == null)
                throw new InvalidOperationException();
            var result = (from XmlElement cdef in crystals
                          let id = int.Parse(cdef.GetAttribute("Id"))
                          let tilePos = new TilePos(int.Parse(cdef.GetAttribute("Left")), int.Parse(cdef.GetAttribute("Top")))
                          let position = tilePos.ToPosition()
                          let score = int.Parse(cdef.GetAttribute("Score"))
                          let energy = int.Parse(cdef.GetAttribute("Energy"))
                          select new Crystal(world, position, id, score, energy)).ToList();
            return result;
            }
        
        public IEnumerable<ForceField> GetListOfForceFields(World world)
            {
            var result = new List<ForceField>();
            var forceFields = this._xmlDoc.SelectNodes("World/ForceFields/Item");
            if (forceFields == null)
                throw new InvalidOperationException();
            foreach (XmlElement fdef in forceFields)
                {
                Rectangle r = WorldArea.GetRectangleFromDefinition(fdef);
                int crystalRequired = int.Parse(fdef.GetAttribute("CrystalRequired"));
                foreach (TilePos p in r.PointsInside())
                    {
                    Vector2 position = p.ToPosition();
                    var f = new ForceField(world, position, crystalRequired);
                    result.Add(f);
                    }
                }
            return result;
            }

        public IEnumerable<Fruit> GetListOfFruit(World world, Tile[,] tiles)
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

        public Tile[,] LoadLayout(List<StaticItem> walls, World world)
            {
            var worldDef = (XmlElement)this._xmlDoc.SelectSingleNode("World");
            if (worldDef == null)
                throw new InvalidOperationException("No World element found.");
            int width = int.Parse(worldDef.GetAttribute("Width"));
            int height = int.Parse(worldDef.GetAttribute("Height"));
            var layout = (XmlElement) this._xmlDoc.SelectSingleNode("World/Layout");
            if (layout == null)
                throw new InvalidOperationException("No Layout element found.");
            var tiles = new Tile[width, height];            
            
            var lines = layout.InnerText.Trim().Replace("\r\n", "\t").Split('\t');
            if (lines.GetLength(0) != height)
                throw new InvalidOperationException();
            
            for (int y = 0; y < height; y++)
                {
                lines[y] = lines[y].Trim();
                if (lines[y].Length != width)
                    throw new InvalidOperationException();
                }
                
            foreach (WorldArea wa in this._worldAreas)
                {
                if (wa.TileDefinitions.Count == 0)
                    continue;
                
                TileDefinition defaultFloor;
                var floorTiles = 
                    (from TileDefinition td in wa.TileDefinitions.Values
                    where td.TileTypeByMap == TileTypeByMap.Floor
                    select td).ToList();
                if (floorTiles.Count == 1)
                    defaultFloor = floorTiles[0];
                else
                    throw new InvalidOperationException();
                    
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
                            floor = GetTexture(defaultFloor.TextureName);
                            tiles[p.X, p.Y] = new Tile(floor, td.TileTypeByMap);
                            var position = new Vector2(p.X * Tile.Width + (Tile.Width / 2), p.Y * Tile.Height + (Tile.Height / 2));
                            var wall = new Wall(world, position, GetTexture(td.TextureName));
                            walls.Add(wall);
                            break;
                        case TileTypeByMap.Floor:
                            floor = GetTexture(td.TextureName);
                            tiles[p.X, p.Y] = new Tile(floor, td.TileTypeByMap);
                            break;
                        case TileTypeByMap.PotentiallyOccupied:
                            floor = GetTexture(defaultFloor.TextureName);
                            tiles[p.X, p.Y] = new Tile(floor, symbol);  
                            break;
                        default:
                            throw new InvalidOperationException();
                        }
                    }
                }
            
            return tiles;
            }
        }
    }
