using System;
using JetBrains.Annotations;
using Labyrinth.Services.WorldBuilding;

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

        [MustUseReturnValue]
        public bool Test(byte mask)
            {
            var result = (this._random.Next(256) & mask) == 0;
            return result;
            }

        [MustUseReturnValue]
        public int Next(int maxValue)
            {
            var result = this._random.Next(maxValue);
            return result;
            }

        [MustUseReturnValue]
        public int DiceRoll([NotNull] DiceRoll diceRoll)
            {
            if (diceRoll == null) 
                throw new ArgumentNullException(nameof(diceRoll));

            var result = 0;
            for (int i = 0; i < diceRoll.NumberOfDice; i++)
                result += this.Next(diceRoll.NumberOfSides) + 1;
            return result;
            }
        }
    }
