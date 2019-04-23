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
        Object
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
        None,
        Vertical,
        Horizontal
        }

    public enum Motion
        {
        Still = 0,
        Forwards = 1,
        Backwards = -1
        }

    /// <summary>
    /// Describes how solid an object is
    /// </summary>
    public enum ObjectSolidity
        {
        /// <summary>
        /// An object that is typically on the move like the player. Any number of insubstantial objects can occupy the same space.
        /// </summary>
        Insubstantial,
        
        /// <summary>
        /// An object that does not move such as a crystal, grave or stationary monster. Only a single stationary object can occupy a given space. Insubstantial objects can move into the same space though.
        /// </summary>
        Stationary,

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
        /// An object that can push another object in the same direction or cause it to bounce backwards (i.e. player)
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

    public enum MovementType
        {
        /// <summary>
        /// Object should move at it's usual speed
        /// </summary>
        Normal,

        /// <summary>
        /// Object should move due to being pushed
        /// </summary>
        Pushed,

        /// <summary>
        /// Object should move due to being bounced back
        /// </summary>
        BounceBack
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

    public enum WorldReturnType
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
        Stationary,

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
        /// Evaluates risk of battle with Player and attacks or retreats appropriately
        /// </summary>
        Cautious
        }

    public enum ChangeRooms
        {
        /// <summary>
        /// Monster always stays in the same room
        /// </summary>
        /// <remarks>Its movement boundaries are the room it inhabits</remarks>
        StaysWithinRoom,

        /// <summary>
        /// Monster is capable of moving to a different room
        /// </summary>
        /// <remarks>Its movement boundaries are the world, but it only "sees" the player when they are in the same room</remarks>
        MovesRoom,

        /// <summary>
        /// Monster will follow the player, if the player leaves the room the monster's in
        /// </summary>
        /// <remarks>Its movement boundaries are the world, and it "sees" the player across room boundaries and can move towards it</remarks>
        FollowsPlayer
        }

    public enum FiringState
        {
        /// <summary>
        /// Cannot or does not fire
        /// </summary>
        None,

        /// <summary>
        /// Fires single rounds
        /// </summary>
        Pulse,

        /// <summary>
        /// Fires in streams
        /// </summary>
        Continuous
        }

    public enum EffectOfShot
        {
        /// <summary>
        /// No interaction - the shot passes through the object
        /// </summary>
        Intangible,

        /// <summary>
        /// The shot is dissipated with no effect on the object
        /// </summary>
        Impervious,

        /// <summary>
        /// The shot causes the object to lose energy and the shot to dissipate
        /// </summary>
        Injury,

        /// <summary>
        /// The shot bounces off the object (unless it already has bounced off)
        /// </summary>
        Reflection
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
        [SoundInfo(3, true)]                StaticObjectShotAndInjured,         // position of object
        [SoundInfo(1, true)]                ForceFieldShutsDown,                // position of forcefield
        [SoundInfo(3, true)]                MineLaid,                           // position of mine
        [SoundInfo(3, true)]                MineArmed                           // position of mine
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

    public enum FruitPopulationMethod
        {
        /// <summary>
        /// The population of fruit is performed once when the world is created
        /// </summary>
        InitialPopulationOnly,

        /// <summary>
        /// Fruit is gradually populated and kept replenished
        /// </summary>
        GradualPopulation,

        /// <summary>
        /// Fruit is given an initial population and kept replenished
        /// </summary>
        InitialPopulationWithReplenishment
        }

    public enum ShootsAtPlayer
        {
        Immediately,
        OnceInjured
        }
    }
