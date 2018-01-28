namespace Labyrinth
    {
    /// <summary>
    /// Bit of a hack to enable score-keeping
    /// </summary>
    public interface IMonster
        {
        /// <summary>
        /// Returns true if the monster has the specified behaviour
        /// </summary>
        /// <typeparam name="T">Specifies the type of behaviour</typeparam>
        /// <returns>True if the monster currently has the specified behaviour, and false otherwise</returns>
        bool HasBehaviour<T>() where T : IBehaviour;
        
        /// <summary>
        /// Returns true if the monster is currently stationary
        /// </summary>
        bool IsStationary { get; }

        /// <summary>
        /// Returns true if the monster is currently an egg
        /// </summary>
        bool IsEgg { get; }
        }
    }
