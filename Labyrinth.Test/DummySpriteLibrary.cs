﻿using System;
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
                if (!(GlobalServices.Game.Services.GetService(typeof (IGraphicsDeviceService)) is IGraphicsDeviceService graphicsDeviceService))
                    throw new InvalidOperationException("The graphics device service has not been created and registered in the Game.Services collection.");

                if (graphicsDeviceService.GraphicsDevice == null)
                    {
                    if (!(GlobalServices.Game.Services.GetService(typeof (IGraphicsDeviceManager)) is IGraphicsDeviceManager graphicsDeviceManager))
                        throw new InvalidOperationException("The graphics device manager has not been created and registered in the Game.Services collection.");
                    graphicsDeviceManager.CreateDevice();
                    }
                this._graphicsDevice = graphicsDeviceService.GraphicsDevice;
                }
            mutex.ReleaseMutex();

            var result = new Texture2D(this._graphicsDevice, Constants.TileRectangle.Width, Constants.TileRectangle.Height, false, SurfaceFormat.Color);
            return result;
            }
        }
    }
