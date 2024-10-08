﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public class SpriteLibrary : ISpriteLibrary
        {
        private readonly Game _game;

        public SpriteLibrary(Game game)
            {
            this._game = game ?? throw new ArgumentNullException(nameof(game));
            }

        public Texture2D GetSprite(string textureName)
            {
            if (textureName == null)
                throw new ArgumentNullException(nameof(textureName));
            if (string.IsNullOrWhiteSpace(textureName))
                throw new ArgumentException("Invalid texture name.", nameof(textureName));

            var result = this._game.Content.Load<Texture2D>(textureName);
            return result;
            }
        }
    }
