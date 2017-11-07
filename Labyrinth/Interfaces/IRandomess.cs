using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    interface IRandomess
        {
        /// <summary>
        /// Tests whether an 8 bit random number is masked as specified, the result is zero
        /// </summary>
        /// <param name="mask">A mask to apply to the random number</param>
        /// <returns>True if the random number arithmetic And the mask equals zero.</returns>
        bool Test(byte mask);

        /// <summary>
        /// Gets a random number between 0 and maxValue - 1
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated.</param>
        /// <returns>An integer that is greater than or equal to 0, and less than maxValue.</returns>
        int Next(int maxValue);

        /// <summary>
        /// Returns the result of a simulated roll of dice
        /// </summary>
        /// <param name="diceRoll">The dice to roll</param>
        /// <returns>The sum of the dice rolls</returns>
        int DiceRoll(DiceRoll diceRoll);
        }
    }
