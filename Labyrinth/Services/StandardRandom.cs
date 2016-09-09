using System;

namespace Labyrinth.Services
    {
    class StandardRandom : IRandomess
        {
        private readonly Random _random;

        public StandardRandom()
            {
            this._random = new Random();
            }

        public StandardRandom(int seed)
            {
            this._random = new Random(seed);
            }

        public bool Test(byte mask)
            {
            var result = (this._random.Next(256) & mask) == 0;
            return result;
            }

        public int Next(int maxValue)
            {
            var result = this._random.Next(maxValue);
            return result;
            }

        public int DiceRoll(int numberOfDice, int numberOfSides)
            {
            if (numberOfDice < 1)    
                throw new ArgumentOutOfRangeException("numberOfDice");
            if (numberOfSides <= 1)    
                throw new ArgumentOutOfRangeException("numberOfSides");

            var result = 0;
            for (int i = 0; i < numberOfDice; i++)
                result += this.Next(numberOfSides) + 1;
            return result;
            }
        }
    }
