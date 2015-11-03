using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteLibrary : ISpriteLibrary
        {
        private ContentManager _contentManager;

        public Texture2D GetSprite(string textureName)
            {
            if (textureName == null)
                throw new ArgumentNullException("textureName");
            if (string.IsNullOrWhiteSpace(textureName))
                throw new ArgumentException("textureName");

            if (this._contentManager == null)
                this._contentManager = new ContentManager(GlobalServices.ServiceProvider, "Content");
            var result = this._contentManager.Load<Texture2D>(textureName);
            return result;
            }

        public void Dispose()
            {
            if (this._contentManager != null)
                {
                this._contentManager.Dispose();
                this._contentManager = null;
                }
            }
        }
    }
