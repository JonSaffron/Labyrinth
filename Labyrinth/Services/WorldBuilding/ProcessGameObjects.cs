using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Labyrinth.DataStructures;
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

        public void AddWalls(IEnumerable<TileDefinitionCollection> tileDefinitionCollections, string[] layout)
            {
            foreach (var tdc in tileDefinitionCollections)
                {
                foreach (TilePos p in tdc.Area.PointsInside())
                    {
                    char symbol = layout[p.Y][p.X];
                    var td = tdc[symbol];
                    if (td is TileWallDefinition wall)
                        { 
                        var pathToTexture = wall.TextureName.Contains("/") ? wall.TextureName : "Tiles/" + wall.TextureName;
                        this._gameState.AddWall(p.ToPosition(), pathToTexture);
                        }
                    }
                }
            }

        public void AddPlayerAndStartPositions(IEnumerable<PlayerStartState> playerStartStates)
            {
            bool playerAdded = false;
            foreach (var pss in playerStartStates)
                {
                if (pss.IsInitialArea)
                    {
                    if (playerAdded)
                        throw new InvalidOperationException("Player has already been added.");
                    this._gameState.AddPlayer(pss.Position.ToPosition(), pss.Energy, pss.Id);
                    playerAdded = true;
                    }
                else
                    {
                    this._gameState.AddTileReservation(pss.Position.ToPosition());
                    }
                }
            }

        public void AddGameObjects(IEnumerable<XmlElement> objectList, XmlNamespaceManager namespaceManager)
            {
            var objectCreationHandlers = BuildMapForObjectCreation(namespaceManager);
            foreach (XmlElement definition in objectList)
                {
                if (!objectCreationHandlers.TryGetValue(definition.LocalName, out var action))
                    {
                    throw new InvalidOperationException("Unknown object type: " + definition.LocalName);
                    }
                action(definition);
                }
            }
        
        private Dictionary<string, Action<XmlElement>> BuildMapForObjectCreation(XmlNamespaceManager namespaceManager)
            {
            var result = new Dictionary<string, Action<XmlElement>>
                {
                    {nameof(Boulder), AddBoulder},
                    {nameof(Monster), item =>
                        {
                        AddMonster(item, namespaceManager);
                        }
                    },
                    {nameof(Crystal), AddCrystal},
                    {nameof(ForceField), AddForceFields},
                    {nameof(CrumblyWall), AddCrumblyWall}
                };
            return result;
            }

        private void AddMonster(XmlElement monsterDef, XmlNamespaceManager namespaceManager)
            {
            MonsterDef md = MonsterDef.FromXml(monsterDef, namespaceManager);
            var tilePos = new TilePos(int.Parse(monsterDef.GetAttribute("Left")), int.Parse(monsterDef.GetAttribute("Top")));
            md.Position = tilePos.ToPosition();
            this._gameState.AddMonster(md);
            }

        private void AddCrystal(XmlElement crystalDef)
            {
            var id = int.Parse(crystalDef.GetAttribute("Id"));
            var tilePos = new TilePos(int.Parse(crystalDef.GetAttribute("Left")), int.Parse(crystalDef.GetAttribute("Top")));
            var position = tilePos.ToPosition();
            var score = int.Parse(crystalDef.GetAttribute("Score"));
            var energy = int.Parse(crystalDef.GetAttribute("Energy"));
            this._gameState.AddCrystal(position, id, score, energy);
            }

        private void AddBoulder(XmlElement boulderDef)
            {
            var tilePos = new TilePos(int.Parse(boulderDef.GetAttribute("Left")), int.Parse(boulderDef.GetAttribute("Top")));
            var position = tilePos.ToPosition();
            this._gameState.AddBoulder(position);
            }

        private void AddForceFields(XmlElement forceFieldDef)
            {
            int crystalRequired = int.Parse(forceFieldDef.GetAttribute("CrystalRequired"));
            Rectangle r = RectangleExtensions.GetRectangleFromDefinition(forceFieldDef);
            foreach (var tp in r.PointsInside())
                {
                this._gameState.AddForceField(tp.ToPosition(), crystalRequired);
                }
            }

        private void AddCrumblyWall(XmlElement wallDef)
            {
            var tilePos = new TilePos(int.Parse(wallDef.GetAttribute("Left")), int.Parse(wallDef.GetAttribute("Top")));
            var position = tilePos.ToPosition();
            var energy = int.Parse(wallDef.GetAttribute("Energy"));
            var textureName = wallDef.GetAttribute("Texture");
            this._gameState.AddCrumblyWall(position, "Tiles/" + textureName, energy);
            }

        public void AddMonstersFromRandomDistributions(IEnumerable<RandomMonsterDistribution> randomMonsterDistributions)
            {
            if (randomMonsterDistributions == null) throw new ArgumentNullException(nameof(randomMonsterDistributions));
            var rnd = GlobalServices.Randomness;
            foreach (var dist in randomMonsterDistributions)
                {
                for (int i = 0; i < dist.CountOfMonsters; i++)
                    {
                    var monsterIndex = rnd.DiceRoll(dist.DiceRoll);
                    var monsterDef = dist.Templates[monsterIndex];
                    monsterDef.Position = GetFreeTile(dist.Area).ToPosition();
                    this._gameState.AddMonster(monsterDef);
                    }
                }
            }

        public void AddFruitFromRandomDistributions(IEnumerable<RandomFruitDistribution> randomFruitDistributions)
            {
            if (randomFruitDistributions == null) throw new ArgumentNullException(nameof(randomFruitDistributions));
            foreach (var dist in randomFruitDistributions)
                {
                foreach (var def in dist.Definitions)
                    {
                    for (int i = 0; i < def.Quantity; i++)
                        {
                        var position = GetFreeTile(dist.Area).ToPosition();
                        this._gameState.AddFruit(position, def.Type, def.Energy);
                        }
                    }
                }
            }

        private TilePos GetFreeTile(Rectangle area)
            {
            var rnd = GlobalServices.Randomness;
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
