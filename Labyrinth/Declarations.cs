namespace Labyrinth
    {
    public delegate void SoundEffectFinished(object sender, SoundEffectFinishedEventArgs args);

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
        /// Tile is expected to be occupied by an object
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

    public enum ShotType
        {
        Player,
        Monster
        }

    public enum BangType
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
    public enum ObjectSolidity
        {
        /// <summary>
        /// An object that does not move such as a crystal. Only a single stationary object can occupy a given space. Insubstatial objects can move into the same space though.
        /// </summary>
        Stationary,

        /// <summary>
        /// An object that is typically on the move like the player. Any number of insubstantial objects can occupy the same space.
        /// </summary>
        Insubstantial,
        
        /// <summary>
        /// A solid object that can be moved such as the boulder. It may occupy the same position as a stationary object.
        /// </summary>
        Moveable,

        /// <summary>
        /// A solid object such as a wall. Only a single object impassable object can occupy a given position.
        /// </summary>
        Impassable
        }

    /// <summary>
    /// Describes what an object is capable of doing to another object
    /// </summary>
    public enum ObjectCapability
        {
        /// <summary>
        /// An object that cannot move another object
        /// </summary>
        CannotMoveOthers,

        /// <summary>
        /// An object that can push another object in the same direction (i.e. shot)
        /// </summary>
        CanPushOthers,

        /// <summary>
        /// An object that can push another objecct in the same direction or cause it to bounce backwards (i.e. player)
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

    public enum LevelReturnType
        {
        Normal,
        LostLife,
        FinishedLevel
        }

    public enum MonsterMobility
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

    public enum ChangeRooms
        {
        StaysWithinRoom,
        MovesRoom,
        FollowsPlayer
        }

    public enum MonsterShootBehaviour
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

    public enum FiringState
        {
        None,
        Pulse,
        Continuous
        }

    public enum GameSound
        {
        BoulderBounces,                     // position of boulder
        PlayerCollectsCrystal,              // single instance
        PlayerCollidesWithMonster,          // position of bang
        PlayerShootsAndInjuresEgg,          // position of egg
        PlayerEatsFruit,                    // no position
        PlayerFinishesWorld,                // single instance
        MonsterDies,                        // position of bang
        MonsterEntersRoom,                  // position of monster
        EggHatches,                         // position of egg
        PlayerShootsAndInjuresMonster,      // position of monster
        MonsterLaysEgg,                     // position of monster
        MonsterLaysMushroom,                // position of monster
        MonsterLeavesRoom,                  // position of monster
        MonsterShoots,                      // position of monster
        PlayerEntersNewLevel,               // single instance
        PlayerDies,                         // single instance
        PlayerInjured,                      // no position
        PlayerMovesFirstFoot,               // single instance
        PlayerMovesSecondFoot,              // single instance
        PlayerShoots,                       // no position
        ShotBounces,                        // position of shot
        MonsterShattersIntoNewLife,         // position of bang
        PlayerStartsNewLife,                // single instance
        StaticObjectShotAndInjured          // position of object
        }

    /// <summary>
    /// Lower number items will be drawn first.
    /// </summary>
    public enum SpriteDrawOrder
        {
        Wall = 0,
        StaticItem = 1,
        ForceField = 2,
        StaticMonster = 3,
        MovingMonster = 4,
        Player = 5,
        Boulder = 6,
        Shot = 7,
        Bang = 8
        }
    }
