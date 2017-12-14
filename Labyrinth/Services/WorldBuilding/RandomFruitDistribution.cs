using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public class RandomFruitDistribution : IHasArea
        {
        public Rectangle Area { get; set; }
        private readonly Dictionary<FruitType, FruitDefinition> _definitions = new Dictionary<FruitType, FruitDefinition>();
        public FruitPopulationMethod PopulationMethod;
        
        public void Add([NotNull] FruitDefinition fd)
            {
            if (fd == null) throw new ArgumentNullException(nameof(fd));
            this._definitions.Add(fd.FruitType, fd);
            }

        public IEnumerable<FruitDefinition> Definitions => this._definitions.Values;

        public FruitDefinition this[FruitType fruitType]
            {
            get
                {
                if (!this._definitions.TryGetValue(fruitType, out var fd))
                    fd = new FruitDefinition {Energy = 0, FruitQuantity = 0, FruitType = fruitType};
                return fd;
                }
            }

        public static RandomFruitDistribution FromXml(XmlElement fruitDistribution)
            {
            var result = new RandomFruitDistribution();
            if (!Enum.TryParse(fruitDistribution.GetAttribute("PopulationMethod"), out FruitPopulationMethod populationMethod))
                {
                throw new InvalidOperationException("Invalid PopulationMethod value.");
                }
            result.PopulationMethod = populationMethod;
            foreach (XmlElement fruitDef in fruitDistribution.ChildNodes)
                {
                var fd = FruitDefinition.FromXml(fruitDef);
                result.Add(fd);
                }
            return result;
            }
        }
    }
