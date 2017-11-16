using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    internal class ProcessGameObjects
        {
        private readonly GameState _gameState;

        public ProcessGameObjects(GameState gameState)
            {
            this._gameState = gameState;
            }

        public void AddWalls(IEnumerable<WorldArea> worldAreas, string[] layout)
            {
            foreach (WorldArea wa in worldAreas)
                {
                var tileDefs = wa.TileDefinitions;
                if (tileDefs == null || tileDefs.Count == 0)
                    continue;
                
                foreach (TilePos p in wa.Area.PointsInside())
                    {
                    char symbol = layout[p.Y][p.X];
                    if (!tileDefs.TryGetValue(symbol, out var td))
                        {
                        string text = $"Don't know what symbol {symbol} indicates in world area {(wa.Id.HasValue ? (object) wa.Id.Value : "(no number)")}";
                        throw new InvalidOperationException(text);
                        }
                    if (td is TileWallDefinition wall)
                        { 
                        this._gameState.AddWall(p.ToPosition(), "Tiles/" + wall.TextureName);
                        }
                    }
                }
            }

        public void AddPlayerAndStartPositions(IEnumerable<WorldArea> worldAreas)
            {
            bool playerAdded = false;
            foreach (var wa in worldAreas)
                {
                if (wa.IsInitialArea)
                    {
                    if (wa.PlayerStartState == null)
                        throw new InvalidOperationException("Initial world area should have a player start state.");
                    if (playerAdded)
                        throw new InvalidOperationException("Player has already been addded.");
                    this._gameState.AddPlayer(wa.PlayerStartState.Position.ToPosition(), wa.PlayerStartState.Energy, wa.Id);
                    playerAdded = true;
                    }
                else if (wa.PlayerStartState != null)
                    {
                    this._gameState.AddTileReservation(wa.PlayerStartState.Position.ToPosition());
                    }
                }
            }

        public void AddGameObjects(IEnumerable<XmlElement> objectList)
            {
            var objectCreationHandlers = BuildMapForObjectCreation();
            foreach (XmlElement definition in objectList)
                {
                if (!objectCreationHandlers.TryGetValue(definition.LocalName, out var action))
                    {
                    throw new InvalidOperationException("Unknown object type: " + definition.LocalName);
                    }
                action(definition);
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

        private void AddMonster(XmlElement mdef)
            {
            MonsterDef md = MonsterDef.FromXml(mdef);
            var tilePos = new TilePos(int.Parse(mdef.GetAttribute("Left")), int.Parse(mdef.GetAttribute("Top")));
            md.Position = tilePos.ToPosition();
            this._gameState.AddMonster(md);
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
            Rectangle r = RectangleExtensions.GetRectangleFromDefinition(fdef);
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

        public void AddMonstersFromRandomDistribution(IEnumerable<WorldArea> worldAreas)
            {
            var rnd = GlobalServices.Randomess;
            var dists = 
                worldAreas.Where(wa => wa.RandomMonsterDistribution != null)
                    .Select(wa => new { wa.Area, Dist = wa.RandomMonsterDistribution});
            foreach (var item in dists)
                {
                for (int i = 0; i < item.Dist.CountOfMonsters; i++)
                    {
                    var monsterIndex = rnd.DiceRoll(item.Dist.DiceRoll);
                    var monsterDef = item.Dist.Templates[monsterIndex];

                    TilePos tp = GetFreeTile(item.Area);
                    monsterDef.Position = tp.ToPosition();
                    this._gameState.AddMonster(monsterDef);

                    i++;
                    }
                }
            }

        public void AddFruit(IEnumerable<WorldArea> worldAreas)
            {
            var fruitDefinitions =
                worldAreas.Where(wa => wa.FruitDefinitions != null)
                    .SelectMany(wa => wa.FruitDefinitions.Values, (wa, fd) => new {wa.Area, FruitDefinition = fd});
            foreach (var item in fruitDefinitions)
                {
                for (int i = 0; i < item.FruitDefinition.FruitQuantity;)
                    {
                    TilePos tp = GetFreeTile(item.Area);
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
                var tp = new TilePos(area.X + rnd.Next(area.Width), area.Y + rnd.Next(area.Height));
                if (!this._gameState.GetItemsOnTile(tp).Any())
                    {
                    return tp;
                    }
                }
            }
        }
    }