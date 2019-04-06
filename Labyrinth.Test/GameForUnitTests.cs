using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class GameForUnitTests : Game
        {
        private World _world;

        public GameForUnitTests([NotNull] IServiceSetup services)
            {
            services.Setup(this);
            GlobalServices.SetGame(this);


            }

        public void LoadLevel(string worldData)
            {
            var gameStartParameters = new GameStartParameters();
            gameStartParameters.World = worldData;

            var gamePlayScreen = new GameplayScreen(gameStartParameters, new InputState());
            this._world = gamePlayScreen.LoadWorld(worldData);
            }

        protected override void Update(GameTime gameTime)
            {
            this._world.Update(gameTime);
            }
        }
    }
