using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects
    {
    public static class GameObjectProperties
        {
        /// <summary>
        /// Determines the order that objects are drawn in
        /// </summary>
        /// <remarks>Objects with a low DrawOrder are drawn before those with a higher DrawOrder.
        /// Objects with the same DrawOrder may be drawn in any order.</remarks>
        public static readonly PropertyDef<int> DrawOrder = new PropertyDef<int>(nameof(DrawOrder), 0);

        /// <summary>
        /// An indication of how solid the object is
        /// </summary>
        /// <remarks>This is used to determine whether objects can share space and how they may interact</remarks>
        public static readonly PropertyDef<ObjectSolidity> Solidity = new PropertyDef<ObjectSolidity>(nameof(Solidity), ObjectSolidity.Stationary);

        /// <summary>
        /// An indication of what an object is capable of doing to another object
        /// </summary>
        public static readonly PropertyDef<ObjectCapability> Capability = new PropertyDef<ObjectCapability>(nameof(Capability), ObjectCapability.CannotMoveOthers);

        /// <summary>
        /// An indication of what happens when an object is shot
        /// </summary>
        public static readonly PropertyDef<EffectOfShot> EffectOfShot = new PropertyDef<EffectOfShot>(nameof(EffectOfShot), Labyrinth.EffectOfShot.Injury);

        /// <summary>
        /// Indicates whether the player scores for killing the monster
        /// </summary>
        public static readonly PropertyDef<bool> MonsterScoresWhenKilled = new PropertyDef<bool>(nameof(MonsterScoresWhenKilled), false);

        // todo effect of forcefield on player
        // todo filter for gameobject solidity
        // public static readonly PropertyDef<Func<bool>> hello = new PropertyDef<Func<bool>>("hello", () => true);

        }
    }
