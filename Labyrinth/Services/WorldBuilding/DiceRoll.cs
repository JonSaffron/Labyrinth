using System;
using System.Text.RegularExpressions;

namespace Labyrinth.Services.WorldBuilding
    {
    internal class DiceRoll
        {
        public int NumberOfDice { get; }
        public int NumberOfSides { get; }

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
                throw new FormatException("Parameter must be in form <nuberofdice>D<numberofsides>, e.g. 4D6.");

            var numberOfDice = int.Parse(match.Groups["numberOfDice"].Value);
            var numberOfSides = int.Parse(match.Groups["numberOfSides"].Value);
            if (numberOfDice <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDice));
            if (numberOfSides <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfSides));
            this.NumberOfDice = numberOfDice;
            this.NumberOfSides = numberOfSides;
            }
        }
    }
