using JetBrains.Annotations;
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

    public enum Orientation
        {
        Vertical,
        Horizontal,
        None
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

    public enum PushStatus
        {
        /// <summary>
        /// The push succeeds and the the pushed object moves in forward direction
        /// </summary>
        Yes,

        /// <summary>
        /// The push fails and nothing can move
        /// </summary>
        No,

        /// <summary>
        /// The push results in a bounce-back and the pushed object moves backwards
        /// </summary>
        Bounce
        }

    public enum FruitType
        {
        [UsedImplicitly] Apple = 1,
        [UsedImplicitly] Watermelon = 2,
        [UsedImplicitly] Pineapple = 3,
        [UsedImplicitly] Strawberry = 4,
        [UsedImplicitly] Cherries = 5,
        [UsedImplicitly] Acorn = 6
        }

    public enum LevelReturnType
        {
        Normal,
        LostLife,
        FinishedWorld
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
        /// <summary>
        /// Monster always stays in the same room
        /// </summary>
        StaysWithinRoom,

        /// <summary>
        /// Monster is capable of moving to a different room
        /// </summary>
        MovesRoom,

        /// <summary>
        /// Monster follows the player if the player leaves the room its in
        /// </summary>
        FollowsPlayer
        }

    public enum MonsterShootBehaviour
        {
        /// <summary>
        /// Monster cannot shoot
        /// </summary>
        None,

        /// <summary>
        /// Monster can shoot at the player
        /// </summary>
        ShootsImmediately,

        /// <summary>
        /// Monster can shoot at the player only after it has been shot at
        /// </summary>
        ShootsHavingBeenShot
        }

    public enum MonsterState
        {
        /// <summary>
        /// Monster is in it's normal state
        /// </summary>
        Normal,

        /// <summary>
        /// Monster is a stationery egg and a countdown is running before it hatches
        /// </summary>
        Egg,

        /// <summary>
        /// The countdown to hatching has expired and the monster is about to transition to its normal state
        /// </summary>
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
        Floor = 0,
        Wall = 1,
        StaticItem = 2,
        ForceField = 3,
        StaticMonster = 4,
        MovingMonster = 5,
        Player = 6,
        Boulder = 7,
        Shot = 8,
        Bang = 9
        }
    }
