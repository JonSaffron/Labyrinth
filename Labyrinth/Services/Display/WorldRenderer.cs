using System;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.PathFinder;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

// todo: drawing is quicker if the same texture is used multiple times rather than swapping between different textures - or use a portion of a bigger texture
// with spritebatch in immediate mode we could turn on GraphicsDevice.RenderState.DepthBufferEnable and specify a layerDepth during Draw and then output in texture order
// http://www.shawnhargreaves.com/blog/spritebatch-and-spritesortmode.html
// http://www.shawnhargreaves.com/blog/spritebatch-sorting-part-2.html
// http://www.shawnhargreaves.com/blog/return-of-the-spritebatch-sorting-part-3.html

namespace Labyrinth.Services.Display
    {
    internal class WorldRenderer
        {
        private readonly TileRect _viewOfWorld;
        private readonly ISpriteBatch _spriteBatch;
        private readonly ISpriteLibrary _spriteLibrary;

        public WorldRenderer(TileRect viewOfWorld, ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary)
            {
            this._viewOfWorld = viewOfWorld;
            this._spriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
            this._spriteLibrary = spriteLibrary ?? throw new ArgumentNullException(nameof(spriteLibrary));
            }

        /// <summary>
        /// Draws the floor of the current view
        /// </summary>
        public void RenderFloorTiles(ref Tile[,] tiles)
            {
            DrawParameters drawParameters = default;
            drawParameters.AreaWithinTexture = Constants.TileRectangle;
            drawParameters.Centre = Vector2.Zero;

            for (int j = 0; j < this._viewOfWorld.Height; j++)
                {
                int y = this._viewOfWorld.TopLeft.Y + j;

                for (int i = 0; i < this._viewOfWorld.Width; i++)
                    {
                    int x = this._viewOfWorld.TopLeft.X + i;

                    string textureName = tiles[x, y].TextureName;

                    var pathToTexture = textureName.Contains("/") ? textureName : "Tiles/" + textureName;
                    drawParameters.Texture = this._spriteLibrary.GetSprite(pathToTexture);
                    drawParameters.Position = new Vector2(x, y) * Constants.TileSize;
                    this._spriteBatch.DrawTexture(drawParameters);
                    }
                }
            }

        /// <summary>
        /// Draws all the GameObjects within the current view
        /// </summary>
        public void RenderWorld()
            {
            var itemsToDraw = GlobalServices.GameState.AllItemsInRectangle(this._viewOfWorld);
            var drawQueue = new PriorityQueue<int, IGameObject>(100);
            foreach (var item in itemsToDraw)
                {
                int drawOrder = (int) item.Properties.Get(GameObjectProperties.DrawOrder);
                drawQueue.Enqueue(drawOrder, item);
                }

            while (!drawQueue.IsEmpty)
                {
                var item = drawQueue.Dequeue();

                var renderAnimation = item.RenderAnimation;

                renderAnimation?.Draw(this._spriteBatch, this._spriteLibrary);
                }
            }
        }
    }
