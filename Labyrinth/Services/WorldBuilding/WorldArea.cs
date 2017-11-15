using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    class WorldArea : IEquatable<WorldArea>
        {
        public readonly int? Id;
        public readonly Rectangle Area;
        public readonly bool IsInitialArea;
        [CanBeNull] public readonly PlayerStartState PlayerStartState;
        [CanBeNull] public readonly Dictionary<char, TileDefinition> TileDefinitions;
        [CanBeNull] public readonly Dictionary<FruitType, FruitDefinition> FruitDefinitions;
        [CanBeNull] public readonly RandomMonsterDistribution RandomMonsterDistribution;

        public WorldArea(XmlElement area, XmlNamespaceManager xnm)
            {
            string id = area.GetAttribute("Id");
            if (!string.IsNullOrEmpty(id))
                this.Id = int.Parse(id);
            
            this.Area = RectangleExtensions.GetRectangleFromDefinition(area);
            
            string worldStart = area.GetAttribute("WorldStart");
            this.IsInitialArea = !string.IsNullOrWhiteSpace(worldStart) && bool.Parse(worldStart);
            
            var startPos = (XmlElement) area.SelectSingleNode("ns:PlayerStartState", xnm);
            if (startPos != null)
                {
                var pss = PlayerStartState.FromXml(startPos);
                if (!this.Area.ContainsTile(pss.Position))
                    throw new InvalidOperationException("Invalid player start position - co-ordinate is not within the area.");
                this.PlayerStartState = pss;
                }

            var tileDefinitions = area.SelectNodes("ns:TileDefinitions/ns:*", xnm);
            this.TileDefinitions = LoadTileDefs(tileDefinitions);

            var fruitPopulation = area.SelectNodes("ns:FruitDefinitions/ns:FruitDef", xnm);
            this.FruitDefinitions = fruitPopulation != null ? LoadFruitPopulation(fruitPopulation) : new Dictionary<FruitType, FruitDefinition>();

            var randomMonsterDistribution = (XmlElement) area.SelectSingleNode("ns:RandomMonsterDistribution", xnm);
            this.RandomMonsterDistribution = randomMonsterDistribution != null ? LoadRandomMonsterDistribution(randomMonsterDistribution, xnm) : new RandomMonsterDistribution();
            }

        private static Dictionary<char, TileDefinition> LoadTileDefs(XmlNodeList tiledefs)
            {
            var result = new Dictionary<char, TileDefinition>();
            foreach (XmlElement tiledef in tiledefs)
                {
                string symbol = tiledef.GetAttribute("Symbol");
                switch (tiledef.Name)
                    {
                    case "Wall":
                        { 
                        string texture = tiledef.GetAttribute("Tile");
                        var td = new TileWallDefinition(symbol, texture);
                        result.Add(td.Symbol, td);
                        break;
                        }
                    case "Floor":
                        {
                        string texture = tiledef.GetAttribute("Tile");
                        bool.TryParse(tiledef.GetAttribute("IsDefault"), out var isDefault);
                        var td = new TileFloorDefinition(symbol, texture, isDefault);
                        result.Add(td.Symbol, td);
                        break;
                        }
                    case "Object":
                        {
                        string description = tiledef.GetAttribute("Description");
                        var td = new TileObjectDefinition(symbol, description);
                        result.Add(td.Symbol, td);
                        break;
                        }
                    }
                }
            var countOfDefaultFloorTiles = result.Values.OfType<TileFloorDefinition>().Count(item => item.IsDefault);
            if (countOfDefaultFloorTiles > 1)
                throw new InvalidOperationException("More than one floor tile is marked as the default.");
            if (countOfDefaultFloorTiles == 0 && result.Values.OfType<TileFloorDefinition>().Count() > 1)
                throw new InvalidOperationException("There are more than one floor tiles, and none of them is marked as the default.");
            return result;
            }
        
        private static Dictionary<FruitType, FruitDefinition> LoadFruitPopulation(XmlNodeList fruitDefs)
            {
            var result = new Dictionary<FruitType, FruitDefinition>();
            foreach (XmlElement fruitDef in fruitDefs)
                {
                var fruitType = (FruitType)Enum.Parse(typeof(FruitType), fruitDef.GetAttribute("Type"));
                int fruitQuantity = int.Parse(fruitDef.GetAttribute("Quantity"));
                int energy = int.Parse(fruitDef.GetAttribute("Energy"));
                var fd = new FruitDefinition {FruitType = fruitType, FruitQuantity = fruitQuantity, Energy = energy};
                result.Add(fd.FruitType, fd);
                }

            return result;
            }

        private static RandomMonsterDistribution LoadRandomMonsterDistribution(XmlElement node, XmlNamespaceManager xnm)
            {
            var result = new RandomMonsterDistribution
                {
                DiceRoll = new DiceRoll(node.GetAttribute("DiceToRoll")),
                CountOfMonsters = int.Parse(node.GetAttribute("CountOfMonsters"))
                };
            // ReSharper disable once PossibleNullReferenceException
            foreach (XmlElement mdef in node.SelectNodes("ns:MonsterTemplates/ns:Monster", xnm))
                {
                var md = MonsterDef.FromXml(mdef);
                int matchingDiceRoll = int.Parse(mdef.GetAttribute("MatchingDiceRoll"));
                result.Templates.Add(matchingDiceRoll, md);
                }

            return result;
            }

        public bool Equals(WorldArea other)
            {
            if (other == null)
                return false;
            return this.Area == other.Area;
            }
        }
    }
