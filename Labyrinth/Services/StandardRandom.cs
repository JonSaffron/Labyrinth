using System;
using System.Diagnostics.Contracts;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.Services
    {
    internal class StandardRandom : IRandomness
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

        [Pure]
        public bool Test(byte mask)
            {
            var result = (this._random.Next(256) & mask) == 0;
            return result;
            }

        [Pure]
        public int Next(int maxValue)
            {
            var result = this._random.Next(maxValue);
            return result;
            }

        [Pure]
        public int DiceRoll(DiceRoll diceRoll)
            {
            if (diceRoll == WorldBuilding.DiceRoll.None) 
                throw new ArgumentOutOfRangeException(nameof(diceRoll));

            var result = 0;
            for (int i = 0; i < diceRoll.NumberOfDice; i++)
                result += this.Next(diceRoll.NumberOfSides) + 1;
            return result;
            }
        }
    }
