using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    static class GlobalServices
        {
        private static ISoundPlayer _soundPlayer;
        private static readonly ISoundPlayer NullSoundPlayer = new NullSoundPlayer();

        private static ISpriteLibrary _spriteLibrary;

        private static GameState _gameState;

        private static IScoreKeeper _scoreKeeper;

        private static GameComponentCollection _gameComponentCollection;

        public static ISoundPlayer SoundPlayer
            {
            get { return _soundPlayer ?? NullSoundPlayer; }
            }

        public static void SetSoundPlayer(ISoundPlayer soundPlayer)
            {
            _soundPlayer = soundPlayer;
            }

        public static ISpriteLibrary SpriteLibrary
            {
            get { return _spriteLibrary; }
            }

        public static void SetSpriteLibrary(ISpriteLibrary spriteLibrary)
            {
            _spriteLibrary = spriteLibrary;
            }

        public static GameState GameState
            {
            get { return _gameState; }
            }

        public static void SetGameState(GameState gameState)
            {
            _gameState = gameState;
            }

        public static IScoreKeeper ScoreKeeper
            {
            get { return _scoreKeeper; }
            }

        public static void SetScoreKeeper(IScoreKeeper scoreKeeper)
            {
            _scoreKeeper = scoreKeeper;
            }

        public static GameComponentCollection GameComponentCollection
            {
            get { return _gameComponentCollection; }
            }

        public static void SetGameComponentCollection(GameComponentCollection gameComponentCollection)
            {
            _gameComponentCollection = gameComponentCollection;
            }
        }
    }
