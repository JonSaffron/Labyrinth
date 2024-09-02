using System;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    internal static class GlobalServices
        {
        private static ISoundPlayer? _soundPlayer;
        private static readonly ISoundPlayer NullSoundPlayer = new NullSoundPlayer();

        public static ISoundPlayer SoundPlayer => _soundPlayer ?? NullSoundPlayer;

        public static void SetSoundPlayer(ISoundPlayer soundPlayer)
            {
            _soundPlayer = soundPlayer ?? throw new ArgumentNullException(nameof(soundPlayer));
            }

        //

        private static ISpriteLibrary? _spriteLibrary;

        public static ISpriteLibrary SpriteLibrary
            {
            get
                {
                if (_spriteLibrary == null)
                    throw new InvalidOperationException("SpriteLibrary has not been set.");
                return _spriteLibrary;
                }
            }

        public static void SetSpriteLibrary(ISpriteLibrary spriteLibrary)
            {
            _spriteLibrary = spriteLibrary ?? throw new ArgumentNullException(nameof(spriteLibrary));
            }

        //

        public static GameState GameState
            {
            get
                {
                if (_world == null)
                    throw new InvalidOperationException("World has not been set.");
                return _world.GameState;
                }
            }

        //

        private static Game? _game;

        public static Game Game
            {
            get
                {
                if (_game == null)
                    throw new InvalidOperationException("Game has not been set.");
                return _game;
                }
            }

        public static void SetGame(Game game)
            {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            }

        //

        private static World? _world;

        public static World World
            {
            get
                {
                if (_world == null)
                    throw new InvalidOperationException("World has not been set.");
                return _world;
                }
            }
        
        public static void SetWorld(World world)
            {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            }

        public static void ClearWorld()
            {
            _world = null;
            }

        //

        private static IPlayerInput? _playerInput;

        public static IPlayerInput PlayerInput
            {
            get
                {
                if (_playerInput == null)
                    throw new InvalidOperationException("PlayerInput has not been set.");
                return _playerInput;
                }
            }

        public static void SetPlayerInput(IPlayerInput playerInput)
            {
            _playerInput = playerInput ?? throw new ArgumentNullException(nameof(playerInput));
            }

        //

        private static ICentrePointProvider? _centrePointProvider;
        private static readonly ICentrePointProvider NullCentrePointProvider = new NullCentrePointProvider();
        
        public static ICentrePointProvider CentrePointProvider => _centrePointProvider ?? NullCentrePointProvider;

        public static void SetCentrePointProvider(ICentrePointProvider centrePointProvider)
            {
            _centrePointProvider = centrePointProvider ?? throw new ArgumentNullException(nameof(centrePointProvider));
            }

        //

        private static IRandomness? _randomness;

        public static IRandomness Randomness
            {
            get
                {
                if (_randomness == null)
                    throw new InvalidOperationException("Randomness has not been set.");
                return _randomness;
                }
            }

        public static void SetRandomness(IRandomness randomness)
            {
            _randomness = randomness ?? throw new ArgumentNullException(nameof(randomness));
            }

        //

        private static IBoundMovementFactory? _boundMovementFactory;
        
        public static IBoundMovementFactory BoundMovementFactory
            {
            get
                {
                if (_boundMovementFactory == null)
                    throw new InvalidOperationException("BoundMovementFactory has not been set.");
                return _boundMovementFactory;
                }
            }

        public static void SetBoundMovementFactory(IBoundMovementFactory boundMovementFactory)
            {
            _boundMovementFactory = boundMovementFactory ?? throw new ArgumentNullException(nameof(boundMovementFactory));
            }

        //

        private static IScoreKeeper? _scoreKeeper;

        public static IScoreKeeper ScoreKeeper
            {
            get
                {
                if (_scoreKeeper == null)
                    throw new InvalidOperationException("ScoreKeeper has not been set.");
                return _scoreKeeper;
                }
            }

        public static void SetScoreKeeper(IScoreKeeper scoreKeeper)
            {
            _scoreKeeper = scoreKeeper ?? throw new ArgumentNullException(nameof(scoreKeeper));
            }
        }
    }
