using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    class WorldArea : IEquatable<WorldArea>
        {
        public int? Id; 
        public Rectangle Area { get; }
        public bool IsInitialArea { get; }

        [CanBeNull] public PlayerStartState PlayerStartState { get; }

        [CanBeNull] public Dictionary<char, TileDefinition> TileDefinitions { get; }

        [CanBeNull] public Dictionary<FruitType, FruitDefinition> FruitDefinitions { get; }

        [CanBeNull] public RandomMonsterDistribution RandomMonsterDistribution { get; }

        public WorldArea(XmlElement area, XmlNamespaceManager xnm)
            {
            string id = area.GetAttribute("Id");
            if (!string.IsNullOrEmpty(id))
                this.Id = int.Parse(id);
            
            this.Area = GetRectangleFromDefinition(area);
            
            string worldStart = area.GetAttribute("WorldStart");
            this.IsInitialArea = !string.IsNullOrWhiteSpace(worldStart) && bool.Parse(worldStart);
            
            var startPos = (XmlElement) area.SelectSingleNode("ns:PlayerStartState", xnm);
            if (startPos != null)
                {
                var pss = LoadPlayerStartState(startPos);
                if (!this.Area.ContainsTile(pss.Position))
                    throw new InvalidOperationException("Invalid player start position - co-ordinate is not within the area.");
                this.PlayerStartState = pss;
                }

            var tileDefinitions = area.SelectNodes("ns:TileDefinitions/ns:TileDef", xnm);
            this.TileDefinitions = tileDefinitions != null ? LoadTileDefs(tileDefinitions) : new Dictionary<char, TileDefinition>();


            var fruitPopulation = area.SelectNodes("ns:FruitDefinitions/ns:FruitDef", xnm);
            this.FruitDefinitions = fruitPopulation != null ? LoadFruitPopulation(fruitPopulation) : new Dictionary<FruitType, FruitDefinition>();

            var randomMonsterDistribution = (XmlElement) area.SelectSingleNode("ns:RandomMonsterDistribution", xnm);
            this.RandomMonsterDistribution = randomMonsterDistribution != null ? LoadRandomMonsterDistribution(randomMonsterDistribution, xnm) : new RandomMonsterDistribution();
            }

        public static Rectangle GetRectangleFromDefinition(XmlElement area)
            {
            int x = int.Parse(area.GetAttribute("Left"));
            int y = int.Parse(area.GetAttribute("Top"));
            int width = int.Parse(area.GetAttribute("Width"));
            int height = int.Parse(area.GetAttribute("Height"));
            Rectangle result = new Rectangle(x, y, width, height);
            return result;
            }

        private static PlayerStartState LoadPlayerStartState(XmlElement startPos)
            {
            var result = new PlayerStartState();
            int x = int.Parse(startPos.GetAttribute("Left"));
            int y = int.Parse(startPos.GetAttribute("Top"));
            var p = new TilePos(x, y);
            result.Position = p;
            result.Energy = int.Parse(startPos.GetAttribute("Energy"));
            return result;
            }

        private static Dictionary<char, TileDefinition> LoadTileDefs(XmlNodeList tiledefs)
            {
            var result = new Dictionary<char, TileDefinition>();
            foreach (XmlElement tiledef in tiledefs)
                {
                string symbol = tiledef.GetAttribute("Symbol");
                string tileDefType = tiledef.GetAttribute("Type");
                string texture = tiledef.GetAttribute("Tile");
                var td = new TileDefinition(symbol, tileDefType, texture);
                result.Add(td.Symbol, td);
                }

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
                var md = WorldLoader.ProcessGameObjects.GetMonsterDef(mdef);
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
