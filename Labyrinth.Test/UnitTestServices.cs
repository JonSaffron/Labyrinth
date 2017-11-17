﻿using Labyrinth.Services.Input;
using Labyrinth.Services.ScoreKeeper;
using Labyrinth.Services.Sound;

namespace Labyrinth.Test
    {
    class UnitTestServices : IServiceSetup
        {
        public IWorldLoader WorldLoader { get; }

        public PlayerController PlayerController;

        public UnitTestServices()
            {
            this.WorldLoader = new WorldLoaderForTest();
            }

        public void Setup(Game1 game)
            {
            var gameInput = new GameInput(game);
            GlobalServices.SetGameInput(gameInput);

            this.PlayerController = new PlayerController(game);
            game.Components.Add(this.PlayerController);
            GlobalServices.SetPlayerInput(this.PlayerController);

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
