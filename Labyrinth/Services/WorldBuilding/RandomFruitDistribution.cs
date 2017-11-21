using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public class RandomFruitDistribution
        {
        public Rectangle Area { get; set; }
        private readonly Dictionary<FruitType, FruitDefinition> _definitions = new Dictionary<FruitType, FruitDefinition>();

        public void Add([NotNull] FruitDefinition fd)
            {
            if (fd == null) throw new ArgumentNullException(nameof(fd));
            this._definitions.Add(fd.FruitType, fd);
            }

        public IEnumerable<FruitDefinition> Definitions => this._definitions.Values;

        public static RandomFruitDistribution FromXml(XmlNodeList fruitDefs)
            {
            var result = new RandomFruitDistribution();
            foreach (XmlElement fruitDef in fruitDefs)
                {
                var fd = FruitDefinition.FromXml(fruitDef);
                result.Add(fd);
                }
            return result;
            }
        }
    }
