using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public class RandomFruitDistribution : IHasArea
        {
        public Rectangle Area { get; set; }
        private readonly Dictionary<FruitType, FruitDefinition> _definitions = new Dictionary<FruitType, FruitDefinition>();
        public FruitPopulationMethod PopulationMethod;
        
        public IEnumerable<FruitDefinition> Definitions => this._definitions.Values;

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
                result._definitions.Add(fd.Type, fd);
                }
            return result;
            }
        }
    }
