using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Test
    {
    class DummySpriteLibrary : ISpriteLibrary
        {
        private GraphicsDevice _graphicsDevice;

        public Texture2D GetSprite(string name)
            {
            var mutex = new Mutex(true, "hello");
            mutex.WaitOne();

            if (this._graphicsDevice == null)
                {
                var graphicsDeviceService = GlobalServices.ServiceProvider.GetService(typeof (IGraphicsDeviceService)) as IGraphicsDeviceService;
                if (graphicsDeviceService == null)
                    throw new InvalidOperationException("The graphics device service has not been created and registered in the Game.Services collection.");
                if (graphicsDeviceService.GraphicsDevice == null)
                    {
                    var graphicsDeviceManager = GlobalServices.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as IGraphicsDeviceManager;
                    if (graphicsDeviceManager == null)
                        throw new InvalidOperationException("The graphics device manager has not been created and registered in the Game.Services collection.");
                    graphicsDeviceManager.CreateDevice();
                    }
                this._graphicsDevice = graphicsDeviceService.GraphicsDevice;
                }
            mutex.ReleaseMutex();

            var result = new Texture2D(this._graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            return result;
            }
        }
    }
