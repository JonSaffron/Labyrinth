using System;
using Labyrinth.Annotations;
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

        public static IScoreKeeper ScoreKeeper { get; private set; }

        public static void SetScoreKeeper(IScoreKeeper scoreKeeper)
            {
            ScoreKeeper = scoreKeeper;
            }

        public static GameComponentCollection GameComponentCollection { get; private set; }

        public static void SetGameComponentCollection(GameComponentCollection gameComponentCollection)
            {
            GameComponentCollection = gameComponentCollection;
            }

        public static IWorldLoader WorldLoader { get; private set; }

        public static void SetWorldLoader(IWorldLoader worldLoader)
            {
            WorldLoader = worldLoader;
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

        public static IServiceProvider ServiceProvider { get; private set; }

        public static void SetServiceProvider(IServiceProvider serviceProvider)
            {
            ServiceProvider = serviceProvider;
            }
        }
    }
