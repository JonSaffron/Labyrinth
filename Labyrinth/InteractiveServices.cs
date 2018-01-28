using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
using Labyrinth.Services.Sound;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    class InteractiveServices : IServiceSetup
        {
        public IWorldLoader WorldLoader { get; }

        public InteractiveServices()
            {
            this.WorldLoader = new WorldLoader();
            }

        public void Setup(Game1 game)
            {
            var gameInput = new GameInput(game);
            GlobalServices.SetGameInput(gameInput);

            var playerInput = new PlayerInput(gameInput);
            GlobalServices.SetPlayerInput(playerInput);
            
            var soundLibrary = new SoundLibrary();
            var activeSoundService = new ActiveSoundService();
            var soundPlayer = new SoundPlayer(soundLibrary, activeSoundService);
            GlobalServices.SetSoundPlayer(soundPlayer);

            var spriteLibrary = new SpriteLibrary(game);
            GlobalServices.SetSpriteLibrary(spriteLibrary);

            // var scoreKeeper = new ScoreKeeper();
            // GlobalServices.SetScoreKeeper(scoreKeeper);
            }
        }
    }
