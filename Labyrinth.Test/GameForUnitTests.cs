using System;
using System.Windows.Forms;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.Display;
using Labyrinth.Services.Input;
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

        public void LoadLevel(string worldData)
            {
            var gameStartParameters = new GameStartParameters
                {
                WorldToLoad = worldData, 
                WorldLoader = new WorldLoaderForTest()
                };

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

public class GraphicsDeviceServiceMock : IGraphicsDeviceService
    {
    GraphicsDevice _GraphicsDevice;
    Form HiddenForm;

    public GraphicsDeviceServiceMock()
        {
        HiddenForm = new Form()
            {
            Visible = false,
            ShowInTaskbar = false
            };

        var Parameters = new PresentationParameters()
            {
            BackBufferWidth = 1280,
            BackBufferHeight = 720,
            DeviceWindowHandle = HiddenForm.Handle,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
            };

        _GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, Parameters);
        }

    public GraphicsDevice GraphicsDevice
        {
        get { return _GraphicsDevice; }
        }

    public event EventHandler<EventArgs> DeviceCreated;
    public event EventHandler<EventArgs> DeviceDisposing;
    public event EventHandler<EventArgs> DeviceReset;
    public event EventHandler<EventArgs> DeviceResetting;

    public void Release()
        {            
        _GraphicsDevice.Dispose();
        _GraphicsDevice = null;

        HiddenForm.Close();            
        HiddenForm.Dispose();
        HiddenForm = null;
        }
    } 
