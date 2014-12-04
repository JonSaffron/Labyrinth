namespace Labyrinth
    {
    /// <summary>
    /// How the map describes each tile's usage
    /// </summary>
    public enum TileTypeByMap
        {
        /// <summary>
        /// Tile can be occupied by an object
        /// </summary>
        Floor,

        /// <summary>
        /// Tile is a wall which cannot be occupied by another object
        /// </summary>
        Wall,

        /// <summary>
        /// Tile is marked as being potentially occupied by an object
        /// </summary>
        PotentiallyOccupied
        }
    
    /// <summary>
    /// What sort of object is occupying a tile
    /// </summary>
    public enum TileTypeByData
        {
        /// <summary>
        /// Tile is not occupied by any objects
        /// </summary>
        Free,

        /// <summary>
        /// Tile is occupied by an object that doesn't move
        /// </summary>
        /// <remarks>No more than one static object can occupy a given tile.</remarks>
        Static,

        /// <summary>
        /// Tile is initially occupied by one or more objects that move
        /// </summary>
        /// <remarks>More than one moving object can occupy a given tile.</remarks>
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
    
    /// <summary>
    /// Describes how solid an object is
    /// </summary>
    enum ObjectSolidity
        {
        /// <summary>
        /// An object can move into a space occupied by a passable object
        /// </summary>
        Passable,

        /// <summary>
        /// An object can push a moveable object away and take its place
        /// </summary>
        Moveable,

        /// <summary>
        /// An object cannot move into a space occupied by an impassable object
        /// </summary>
        Impassable
        }

    /// <summary>
    /// Describes what an object is capable of doing to another object
    /// </summary>
    enum ObjectCapability
        {
        /// <summary>
        /// An object that cannot move another object
        /// </summary>
        CannotMoveOthers,

        /// <summary>
        /// An object that can push another object in the same direction
        /// </summary>
        CanPushOthers,

        /// <summary>
        /// An object that can push another objecct in the same direction or cause it to bounce backwards
        /// </summary>
        CanPushOrCauseBounceBack
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
