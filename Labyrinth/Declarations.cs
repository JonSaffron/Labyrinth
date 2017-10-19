using Labyrinth.Services.Sound;

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
        /// An object that does not move such as a crystal or grave. Only a single stationary object can occupy a given space. Insubstatial objects can move into the same space though.
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

    public enum FruitType
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
        [SoundInfo(3, true)]                BoulderBounces,                     // position of boulder
        [SoundInfo]                         PlayerCollectsCrystal,              // single instance
        [SoundInfo(3, true)]                PlayerCollidesWithMonster,          // position of bang
        [SoundInfo(3, true)]                PlayerShootsAndInjuresEgg,          // position of egg
        [SoundInfo(3, true)]                PlayerEatsFruit,                    // position of player
        [SoundInfo]                         PlayerFinishesWorld,                // single instance
        [SoundInfo(3, true)]                MonsterDies,                        // position of bang
        [SoundInfo(3, true)]                MonsterEntersRoom,                  // position of monster
        [SoundInfo(3, true)]                EggHatches,                         // position of egg
        [SoundInfo(3, true)]                PlayerShootsAndInjuresMonster,      // position of monster
        [SoundInfo(3, true)]                MonsterLaysEgg,                     // position of monster
        [SoundInfo(3, true)]                MonsterLaysMushroom,                // position of monster
        [SoundInfo(3, true)]                MonsterLeavesRoom,                  // position of monster
        [SoundInfo(3, true)]                MonsterShoots,                      // position of monster
        [SoundInfo]                         PlayerEntersNewLevel,               // single instance
        [SoundInfo]                         PlayerDies,                         // single instance
        [SoundInfo(3, true)]                PlayerInjured,                      // position of player
        [SoundInfo(1, true, "PlayerMoves")] PlayerMovesFirstFoot,               // single instance at position of player
        [SoundInfo(1, true, "PlayerMoves")] PlayerMovesSecondFoot,              // single instance at position of player
        [SoundInfo(3, true)]                PlayerShoots,                       // position of player
        [SoundInfo(3, true)]                ShotBounces,                        // position of shot
        [SoundInfo(3, true)]                MonsterShattersIntoNewLife,         // position of bang
        [SoundInfo]                         PlayerStartsNewLife,                // single instance
        [SoundInfo(3, true)]                StaticObjectShotAndInjured          // position of object
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
