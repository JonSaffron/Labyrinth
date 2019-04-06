using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Labyrinth.Services.ScoreKeeper;
using Labyrinth.Services.Sound;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class InteractiveServices : IServiceSetup
        {
        public IWorldLoader WorldLoader { get; }
        public IScoreKeeper ScoreKeeper { get; }

        public InteractiveServices()
            {
            this.WorldLoader = new WorldLoader();
            this.ScoreKeeper = new ScoreKeeper();
            }

        public void Setup(Game game)
            {
            //var gameInput = new GameInput(game);
            //GlobalServices.SetGameInput(gameInput);

            //var playerInput = new PlayerInput(gameInput);
            //GlobalServices.SetPlayerInput(playerInput);
            
            var soundLibrary = new SoundLibrary();
            var activeSoundService = new ActiveSoundService();
            var soundPlayer = new SoundPlayer(soundLibrary, activeSoundService);
            GlobalServices.SetSoundPlayer(soundPlayer);

            var spriteLibrary = new SpriteLibrary(game);
            GlobalServices.SetSpriteLibrary(spriteLibrary);

            var monsterFactory = new MonsterFactory();
            GlobalServices.SetMonsterFactory(monsterFactory);
            }
        }
    }
