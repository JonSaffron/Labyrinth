using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    /// <summary>
    /// A class used to determine how a monster moves about the game world
    /// </summary>
    public interface IMonsterMovement
        {
        /// <summary>
        /// Returns the next direction of movement for the specified monster
        /// </summary>
        /// <param name="monster">Specifies the monster who is moving</param>
        /// <returns>The direction the monster will move in (can be None)</returns>
        Direction DetermineDirection([NotNull] Monster monster);
        }
    }
