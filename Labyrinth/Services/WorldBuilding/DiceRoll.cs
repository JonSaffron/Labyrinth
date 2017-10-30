using System;
using System.Text.RegularExpressions;

namespace Labyrinth.Services.WorldBuilding
    {
    class DiceRoll
        {
        public int NumberOfDice { get; }

        public int NumberOfSides { get; }

        public DiceRoll(string diceRoll)
            {
            const string pattern = @"^(?<numberOfDice>\d+)D(?<numberOfSides>\d+)$";
            var regExp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regExp.Match(diceRoll);
            if (!match.Success)
                throw new FormatException("Parameter must be in form <nuberofdice>D<numberofsides>, e.g. 4D6.");

            this.NumberOfDice = int.Parse(match.Groups["numberOfDice"].Value);
            this.NumberOfSides = int.Parse(match.Groups["numberOfSides"].Value);
            }
        }
    }
