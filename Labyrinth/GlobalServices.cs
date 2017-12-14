using JetBrains.Annotations;
using Labyrinth.Services.Input;
using Labyrinth.Services.ScoreKeeper;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    static class GlobalServices
        {
        private static ISoundPlayer _soundPlayer;
        private static readonly ISoundPlayer NullSoundPlayer = new NullSoundPlayer();

        public static ISoundPlayer SoundPlayer
            {
            get { return _soundPlayer ?? NullSoundPlayer; }
            }

        public static void SetSoundPlayer(ISoundPlayer soundPlayer)
            {
            _soundPlayer = soundPlayer;
            }

        public static ISpriteLibrary SpriteLibrary { get; private set; }

        public static void SetSpriteLibrary(ISpriteLibrary spriteLibrary)
            {
            SpriteLibrary = spriteLibrary;
            }

        public static GameState GameState { get; private set; }

        public static void SetGameState(GameState gameState)
            {
            GameState = gameState;
            }

        private static IScoreKeeper _scoreKeeper;
        private static readonly IScoreKeeper NullScoreKeeper = new NullScoreKeeper();

        public static IScoreKeeper ScoreKeeper
            {
            get
                {
                return _scoreKeeper ?? NullScoreKeeper;
                }
            }

        public static void SetScoreKeeper(IScoreKeeper scoreKeeper)
            {
            _scoreKeeper = scoreKeeper;
            }

        public static Game Game { get; private set; }

        public static void SetGame(Game game)
            {
            Game = game;
            }

        public static World World { get; private set; }
        
        public static void SetWorld(World world)
            {
            World = world;
            }

        public static GameInput GameInput { get; private set; }

        public static void SetGameInput(GameInput gameInput)
            {
            GameInput = gameInput;
            }

        public static IPlayerInput PlayerInput { get; private set; }

        public static void SetPlayerInput(IPlayerInput playerInput)
            {
            PlayerInput = playerInput;
            }

        public static ICentrePointProvider CentrePointProvider { [CanBeNull] get; private set; }

        public static void SetCentrePointProvider(ICentrePointProvider centrePointProvider)
            {
            CentrePointProvider = centrePointProvider;
            }

        public static IMonsterMovementFactory MonsterMovementFactory { get; private set; }

        public static void SetMonsterMovementFactory(IMonsterMovementFactory monsterMovementFactory)
            {
            MonsterMovementFactory = monsterMovementFactory;
            }

        public static IRandomess Randomess { get; private set; }

        public static void SetRandomness(IRandomess randomess)
            {
            Randomess = randomess;
            }
        }
    }
