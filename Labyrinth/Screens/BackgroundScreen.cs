using Microsoft.Xna.Framework;
using System;
using Labyrinth.DataStructures;
using Labyrinth.Services.WorldBuilding;
using Labyrinth.Services.Display;

namespace Labyrinth.Screens
    {
    internal class BackgroundScreen : GameScreen
        {
        // order of these declarations is important - Cy and Cx must be set before the call to BuildBackgroundTiles
        private static readonly int Cy = (int)Constants.RoomSizeInTiles.Y;
        private static readonly int Cx = (int)Constants.RoomSizeInTiles.X;
        private static Tile[,] _tiles = BuildBackgroundTiles();

        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
            {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
            {
            base.Update(gameTime, otherScreenHasFocus, false);
            }

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            ISpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.WindowPosition = Vector2.Zero;
            var tileRectangle = new TileRect(TilePos.Zero, Cx, Cy);

            spriteBatch.Begin();
            WorldRenderer worldRenderer = new WorldRenderer(tileRectangle, spriteBatch, GlobalServices.SpriteLibrary);
            worldRenderer.RenderFloorTiles(ref _tiles);
            spriteBatch.End();
            }

        private static Tile[,] BuildBackgroundTiles()
            {
            var result = new Tile[Cx, Cy];
            for (int y = 0; y < Cy; y++)
                {
                for (int x = 0; x < Cx; x++)
                    {
                    bool isEdge = y == 0 || y == Cy - 1 || x == 0 || x == Cx - 1;
                    result[x, y] = new Tile(isEdge ? "Wall3Full" : "Floor3", 0);
                    }
                }

            return result;
            }
        }
    }
