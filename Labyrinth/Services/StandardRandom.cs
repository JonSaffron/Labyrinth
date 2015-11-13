using System;

namespace Labyrinth.Services
    {
    class StandardRandom : IRandomess
        {
        private readonly Random _random = new Random();

        public bool Test(byte value)
            {
            var result = (this._random.Next(256) & value) == 0;
            return result;
            }

        public int Next(int maxValue)
            {
            var result = this._random.Next(maxValue);
            return result;
            }
        }
    }
