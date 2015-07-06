namespace Labyrinth
    {
    class FruitDefinition
        {
        public FruitType FruitType { get; private set; }
        public int FruitQuantity { get; private set; }
        
        public FruitDefinition(FruitType fruitType, int quantity)
            {
            this.FruitType = fruitType;
            this.FruitQuantity = quantity;
            }
        }
    }
