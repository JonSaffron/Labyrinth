using System;
using System.Xml;

namespace Labyrinth.Services.WorldBuilding
    {
    public class FruitDefinition
        {
        public FruitType FruitType { get; set; }
        public int FruitQuantity { get; set; }
        public int Energy { get; set; }

        public static FruitDefinition FromXml(XmlElement fruitDef)
            {
            var fruitType = (FruitType)Enum.Parse(typeof(FruitType), fruitDef.GetAttribute("Type"));
            int fruitQuantity = int.Parse(fruitDef.GetAttribute("Quantity"));
            int energy = int.Parse(fruitDef.GetAttribute("Energy"));
            var result = new FruitDefinition { FruitType = fruitType, FruitQuantity = fruitQuantity, Energy = energy };
            return result;
            }
        }
    }
