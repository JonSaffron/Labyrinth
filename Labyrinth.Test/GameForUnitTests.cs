using System.Linq;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class GameForUnitTests : Game
        {
        private World _world;
        private readonly UnitTestServices _unitTestServices;

        public GameForUnitTests([NotNull] UnitTestServices services)
            {
            services.Setup(this);
            GlobalServices.SetGame(this);
            this._unitTestServices = services;
            }

        public void LoadLevel(string worldData)
            {
            var gameStartParameters = new GameStartParameters();
            gameStartParameters.World = worldData;
            gameStartParameters.WorldLoader = new WorldLoaderForTest();

            var gamePlayScreen = new GameplayScreen(gameStartParameters, new InputState());
            this._world = gamePlayScreen.LoadWorld(worldData);
            GlobalServices.SetWorld(this._world);
            }

        protected override void Update(GameTime gameTime)
            {
            base.Update(gameTime);
            this._world.Update(gameTime);
            }

        public void RunTest()
            {
            while (!this._unitTestServices.PlayerController.HasFinishedQueue)
                {
                this.Tick();
                }

            for (int i = 0; i < 5; i++)
                {
                this.Tick();
                if (IsAnythingMoving())
                    {
                    i = 0;
                    }
                }
            }

        private static bool IsAnythingMoving()
            {
            foreach (var item in GlobalServices.GameState.GetSurvivingInteractiveItems())
                {
                if (item is IStandardShot)
                    return true;
                if (item is Bang)
                    return true;
                if (item.CurrentMovement.IsMoving)
                    return true;
                }
            return false;
            }
        }
    }
