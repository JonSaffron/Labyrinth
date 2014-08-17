namespace Labyrinth
    {
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    enum TileCollision
        {
        /// <summary>
        /// A passable, empty tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1
        }
    
    public enum TileTypeByMap
        {
        Floor,
        Wall,
        PotentiallyOccupied
        }
    
    public enum TileTypeByData
        {
        Free,
        Static,
        Moving
        }

    public enum TileExceptionType
        {
        Warning,
        Error
        }

    enum ShotType
        {
        Player,
        Monster
        }
    
    enum BangType
        {
        Short,
        Long
        }
    
    public enum Direction
        {
        None,
        Left,
        Right,
        Up,
        Down
        }
        
    enum PushStatus
        {
        Yes,
        No,
        Bounce
        }
    
    enum FruitType
        {
        Apple = 1,
        Watermelon = 2,
        Pineapple = 3,
        Strawberry = 4,
        Cherries = 5,
        Acorn = 6
        }
    
    enum ShotStatus
        {
        HitHome,        // hit and hurt monster, or impassable tile
        BounceOff,      // bounce off monster or object
        CarryOn         // nothing in the way
        }
    
    enum ShotOutcome
        {
        Remove,
        Continue,
        ConvertToBang
        }
    
    enum LevelReturnType
        {
        Normal,
        LostLife,
        FinishedLevel
        }
    
    enum MonsterMobility
        {
        /// <summary>
        /// Not moving
        /// </summary>
        Static,

        /// <summary>
        /// Moving without reference to the player's position
        /// </summary>
        Placid,

        /// <summary>
        /// Moving in a directly threatening way towards the player
        /// </summary>
        Aggressive,

        /// <summary>
        /// Following a set path without reference to the player's position
        /// </summary>
        Patrolling,

        /// <summary>
        /// Moving towards or away from the player
        /// </summary>
        Cautious
        }
    
    enum TouchResult
        {
        NoEffect,
        RemoveObject,
        BounceBack
        }
    
    enum ChangeRooms
        {
        StaysWithinRoom,
        MovesRoom,
        FollowsPlayer
        }
    
    enum MonsterShootBehaviour
        {
        None,
        ShootsImmediately,
        ShootsHavingBeenShot
        }

    public enum MonsterState
        {
        Normal,
        Egg,
        Hatching
        }

    public enum InjuryType
        {
            
        }

    public enum GameSound
        {
        BoulderBounces,
        PlayerCollectsCrystal,
        PlayerCollidesWithMonster,
        PlayerShootsAndInjuresEgg,
        PlayerEatsFruit,
        PlayerFinishesWorld,
        MonsterDies,
        MonsterEntersRoom,
        EggHatches,
        PlayerShootsAndInjuresMonster,
        MonsterLaysEgg,
        MonsterLaysMushroom,
        MonsterLeavesRoom,
        MonsterShoots,
        PlayerEntersNewLevel,
        PlayerDies,
        PlayerInjured,
        PlayerMoves,
        PlayerShoots,
        ShotBounces,
        MonsterShattersIntoNewLife,
        PlayerStartsNewLife,
        StaticObjectShotAndInjured
        }
    }
