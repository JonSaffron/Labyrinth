using System;
using System.Text.RegularExpressions;

namespace Labyrinth.Services.WorldBuilding
    {
    class DiceRoll
        {
        private readonly int _numberOfDice;
        private readonly int _numberOfSides;

        public DiceRoll(string diceRoll)
            {
            const string pattern = @"^(?<numberOfDice>\d+)D(?<numberOfSides>\d+)$";
            var regExp = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regExp.Match(diceRoll);
            if (!match.Success)
                throw new FormatException("Parameter must be in form <nuberofdice>D<numberofsides>, e.g. 4D6.");

            this._numberOfDice = int.Parse(match.Groups["numberOfDice"].Value);
            this._numberOfSides = int.Parse(match.Groups["numberOfSides"].Value);
            }

        public int NumberOfDice
            {
            get { return this._numberOfDice; }
            }

        public int NumberOfSides
            {
            get { return this._numberOfSides; }
            }
        }
    }
