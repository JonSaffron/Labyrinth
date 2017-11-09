using System;
using Labyrinth.Services.Input;
using Labyrinth.Services.ScoreKeeper;
using Labyrinth.Services.Sound;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.Test
    {
    class UnitTestServices : IServiceSetup
        {
        public IWorldLoader WorldLoader { get; }

        private readonly PlayerController _pc;

        public UnitTestServices(PlayerController pc)
            {
            this.WorldLoader = new WorldLoaderForTest();
            this._pc = pc ?? throw new ArgumentNullException(nameof(pc));
            }

        public void Setup(Game1 game)
            {
            var gameInput = new GameInput(game);
            GlobalServices.SetGameInput(gameInput);

            GlobalServices.SetPlayerInput(this._pc);

            var soundPlayer = new NullSoundPlayer();
            GlobalServices.SetSoundPlayer(soundPlayer);

            var spriteLibrary = new DummySpriteLibrary();
            GlobalServices.SetSpriteLibrary(spriteLibrary);

            var scoreKeeper = new NullScoreKeeper();
            GlobalServices.SetScoreKeeper(scoreKeeper);

            game.Components.Add(new SuppressDrawComponent(game));
            }
        }
    }
