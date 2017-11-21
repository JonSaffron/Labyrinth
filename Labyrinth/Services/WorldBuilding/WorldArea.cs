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
        public readonly Rectangle Area;
        [CanBeNull] public readonly PlayerStartState PlayerStartState;
        [CanBeNull] public readonly TileDefinitionCollection TileDefinitionCollection;
        [CanBeNull] public readonly RandomFruitDistribution FruitDefinitions;
        [CanBeNull] public readonly RandomMonsterDistribution RandomMonsterDistribution;

        public WorldArea(XmlElement area, XmlNamespaceManager xnm)
            {
            
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
            if (tileDefinitions != null)
                {
                var td = TileDefinitionCollection.FromXml(tileDefinitions);
                td.Area = this.Area;
                this.TileDefinitionCollection = td;
                }

            var fruitPopulation = area.SelectNodes("ns:FruitDefinitions/ns:FruitDef", xnm);
            if (fruitPopulation != null)
                {
                var fd = RandomFruitDistribution.FromXml(fruitPopulation);
                fd.Area = this.Area;
                this.FruitDefinitions = fd;
                }

            var randomMonsterDistribution = (XmlElement) area.SelectSingleNode("ns:RandomMonsterDistribution", xnm);
            if (randomMonsterDistribution != null)
                {
                var md = RandomMonsterDistribution.FromXml(randomMonsterDistribution, xnm);
                md.Area = this.Area;
                this.RandomMonsterDistribution = md;
                }
            }

        public bool Equals(WorldArea other)
            {
            if (other == null)
                return false;
            return this.Area == other.Area;
            }
        }
    }
