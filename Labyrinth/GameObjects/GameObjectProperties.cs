namespace Labyrinth.GameObjects
    {
    public static class GameObjectProperties
        {
        public static readonly PropertyDef<int> DrawOrder = new PropertyDef<int>(nameof(DrawOrder), 0);
        public static readonly PropertyDef<ObjectSolidity> Solidity = new PropertyDef<ObjectSolidity>(nameof(Solidity), ObjectSolidity.Stationary);

        public static readonly PropertyDef<ObjectCapability> Capability = new PropertyDef<ObjectCapability>(nameof(Capability), ObjectCapability.CannotMoveOthers);
        public static readonly PropertyDef<decimal> StandardSpeed = new PropertyDef<decimal>(nameof(StandardSpeed), Constants.BaseSpeed);

        public static readonly PropertyDef<EffectOfShot> EffectOfShot = new PropertyDef<EffectOfShot>(nameof(EffectOfShot), Labyrinth.EffectOfShot.Injury);
        public static readonly PropertyDef<bool> MonsterScoresWhenKilled = new PropertyDef<bool>(nameof(MonsterScoresWhenKilled), false);
        }
    }
