using System;
using System.Text.RegularExpressions;

namespace Labyrinth.Services.WorldBuilding
    {
    public readonly struct DiceRoll : IEquatable<DiceRoll>
        {
        public int NumberOfDice { get; }
        public int NumberOfSides { get; }

        public static DiceRoll None = new DiceRoll();

        public DiceRoll(int numberOfDice, int numberOfSides)
            {
            if (numberOfDice <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDice));
            if (numberOfSides <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfSides));
            this.NumberOfDice = numberOfDice;
            this.NumberOfSides = numberOfSides;
            }

        public DiceRoll(string diceRoll)
            {
            const string pattern = @"^(?<numberOfDice>\d+)D(?<numberOfSides>\d+)$";
            var regExp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regExp.Match(diceRoll);
            if (!match.Success)
                throw new FormatException("Parameter must be in form <numberOfDice>D<numberOfSides>, e.g. 4D6.");

            var numberOfDice = int.Parse(match.Groups["numberOfDice"].Value);
            var numberOfSides = int.Parse(match.Groups["numberOfSides"].Value);
            if (numberOfDice <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDice));
            if (numberOfSides <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfSides));
            this.NumberOfDice = numberOfDice;
            this.NumberOfSides = numberOfSides;
            }

        public int MinValue => this.NumberOfDice;

        public int MaxValue => this.NumberOfDice * this.NumberOfSides;

        public override bool Equals(object obj)
            {
            return obj is DiceRoll roll && this.Equals(roll);
            }

        public bool Equals(DiceRoll other)
            {
            return this.NumberOfDice == other.NumberOfDice &&
                   this.NumberOfSides == other.NumberOfSides;
            }

        public override int GetHashCode()
            {
            var hashCode = 214213997;
            hashCode = hashCode * -1521134295 + this.NumberOfDice.GetHashCode();
            hashCode = hashCode * -1521134295 + this.NumberOfSides.GetHashCode();
            return hashCode;
            }

        public static bool operator ==(DiceRoll roll1, DiceRoll roll2)
            {
            return roll1.Equals(roll2);
            }

        public static bool operator !=(DiceRoll roll1, DiceRoll roll2)
            {
            return !(roll1 == roll2);
            }
        }
    }
