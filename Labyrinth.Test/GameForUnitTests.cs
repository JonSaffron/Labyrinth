using System;
using System.Windows.Forms;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Test
    {
    class GameForUnitTests : Game
        {
        private World _world;
        public readonly UnitTestServices UnitTestServices;

        public GameForUnitTests()
            {
            this.UnitTestServices = new UnitTestServices();
            this.UnitTestServices.Setup(this);

            this.Services.AddService<IGraphicsDeviceService>(new GraphicsDeviceServiceMock());
            this.Components.Add(new SuppressDrawComponent(this));
            }

        public void LoadWorld(string worldData)
            {
            var worldLoader = new WorldLoaderForTest(worldData);
            var boundMovementFactory = new BoundMovementFactory(worldLoader.WorldSize);
            GlobalServices.SetBoundMovementFactory(boundMovementFactory);

            this._world = new World(worldLoader);
            GlobalServices.SetWorld(_world);

            _world.ResetWorldForStartingNewLife();
            }

        protected override void Update(GameTime gameTime)
            {
            base.Update(gameTime);
            this._world.Update(gameTime);
            }

        public void RunTest()
            {
            while (!this.UnitTestServices.PlayerController.HasFinishedQueue)
                {
                this.RunOneFrame();
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
            foreach (var item in GlobalServices.GameState.GetSurvivingGameObjects())
                {
                if (item is IStandardShot)
                    return true;
                if (item is Bang)
                    return true;
                if (item is IMovingItem { CurrentMovement.IsMoving: true })
                    return true;
                }
            return false;
            }
        }

    public class GraphicsDeviceServiceMock : IGraphicsDeviceService
        {
        private Form _hiddenForm;

        public GraphicsDeviceServiceMock()
            {
            this._hiddenForm = new Form()
                {
                Visible = false,
                ShowInTaskbar = false
                };

            var parameters = new PresentationParameters()
                {
                BackBufferWidth = 1280,
                BackBufferHeight = 720,
                DeviceWindowHandle = this._hiddenForm.Handle,
                PresentationInterval = PresentInterval.Immediate,
                IsFullScreen = false
                };

            this.GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, parameters);
            }

        public GraphicsDevice GraphicsDevice { get; private set; }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public void Release()
            {            
            this.GraphicsDevice.Dispose();
            this.GraphicsDevice = null;

            this._hiddenForm.Close();            
            this._hiddenForm.Dispose();
            this._hiddenForm = null;
            }
        } 
    }
