using System;
using System.Xml;

namespace Labyrinth.Services.WorldBuilding
    {
    public readonly struct FruitDefinition
        {
        public FruitType Type { get; }
        public int Quantity { get; }
        public int Energy { get; }

        public static FruitDefinition FromXml(XmlElement fruitDef)
            {
            var type = (FruitType)Enum.Parse(typeof(FruitType), fruitDef.GetAttribute("Type"));
            int quantity = int.Parse(fruitDef.GetAttribute("Quantity"));
            int energy = int.Parse(fruitDef.GetAttribute("Energy"));
            var result = new FruitDefinition(type, quantity, energy);
            return result;
            }

        private FruitDefinition(FruitType type, int quantity, int energy)
            {
            if (!Enum.IsDefined(typeof(FruitType), type))
                throw new ArgumentOutOfRangeException(nameof(type));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));
            if (energy <= 0)
                throw new ArgumentOutOfRangeException(nameof(energy));

            this.Type = type;
            this.Quantity = quantity;
            this.Energy = energy;
            }
        }
    }
