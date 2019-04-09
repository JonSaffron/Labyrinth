using JetBrains.Annotations;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    static class GlobalServices
        {
        private static ISoundPlayer _soundPlayer;
        private static readonly ISoundPlayer NullSoundPlayer = new NullSoundPlayer();

        public static ISoundPlayer SoundPlayer => _soundPlayer ?? NullSoundPlayer;

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

        public static IRandomness Randomness { get; private set; }

        public static void SetRandomness(IRandomness randomness)
            {
            Randomness = randomness;
            }

        public static void SetBoundMovementFactory(IBoundMovementFactory boundMovementFactory)
            {
            BoundMovementFactory = boundMovementFactory;
            }

        public static IBoundMovementFactory BoundMovementFactory { get; private set; }

        }
    }
