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
        /// Gets an indication of how solid the object is
        /// </summary>
        /// <remarks>This is used to determine whether objects can share space and how they may interact</remarks>
        public static readonly PropertyDef<ObjectSolidity> Solidity = new PropertyDef<ObjectSolidity>(nameof(Solidity), ObjectSolidity.Stationary);

        public static readonly PropertyDef<ObjectCapability> Capability = new PropertyDef<ObjectCapability>(nameof(Capability), ObjectCapability.CannotMoveOthers);
        //public static readonly PropertyDef<decimal> StandardSpeed = new PropertyDef<decimal>(nameof(StandardSpeed), Constants.BaseSpeed);

        public static readonly PropertyDef<EffectOfShot> EffectOfShot = new PropertyDef<EffectOfShot>(nameof(EffectOfShot), Labyrinth.EffectOfShot.Injury);
        public static readonly PropertyDef<bool> MonsterScoresWhenKilled = new PropertyDef<bool>(nameof(MonsterScoresWhenKilled), false);
        }
    }
