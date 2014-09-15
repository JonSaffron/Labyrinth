namespace Labyrinth
    {
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
