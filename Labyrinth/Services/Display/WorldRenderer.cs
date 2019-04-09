using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.PathFinder;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

// todo drawing is quicker if the same texture is used multiple times rather than swapping between different textures - or use a portion of a bigger texture
// with spritebatch in immediate mode we could turn on GraphicsDevice.RenderState.DepthBufferEnable and specify a layerDepth during Draw and then output in texture order
// http://www.shawnhargreaves.com/blog/spritebatch-and-spritesortmode.html
// http://www.shawnhargreaves.com/blog/spritebatch-sorting-part-2.html
// http://www.shawnhargreaves.com/blog/return-of-the-spritebatch-sorting-part-3.html


namespace Labyrinth.Services.Display
    {
    class WorldRenderer
        {
        private readonly ISpriteLibrary _spriteLibrary;
        private readonly Tile[,] _tiles;


        public WorldRenderer([NotNull] ISpriteLibrary spriteLibrary, ref Tile[,] tiles)
            {
            this._spriteLibrary = spriteLibrary ?? throw new ArgumentNullException(nameof(spriteLibrary));
            this._tiles = tiles;
            }

        public void RenderWorld(GameTime gameTime, TileRect viewOfWorld, [NotNull] ISpriteBatch spriteBatch)
            {
            if (spriteBatch == null) 
                throw new ArgumentNullException(nameof(spriteBatch));

            DrawFloorTiles(viewOfWorld, spriteBatch);

            var itemsToDraw = GlobalServices.GameState.AllItemsInRectangle(viewOfWorld);
            var drawQueue = new PriorityQueue<int, IGameObject>(100);
            foreach (var item in itemsToDraw)
                {
                int drawOrder = item.Properties.Get(GameObjectProperties.DrawOrder);
                drawQueue.Enqueue(drawOrder, item);
                }
            //Trace.WriteLine("Drawing " + drawQueue.Count + " sprites.");
            while (!drawQueue.IsEmpty)
                {
                var item = drawQueue.Dequeue();
                if (!(item is MovingItem))
                    item.Update(gameTime);
                var renderAnimation = item.RenderAnimation;

                renderAnimation?.Draw(spriteBatch, this._spriteLibrary);
                }
            }

        /// <summary>
        /// Draws the floor of the current view
        /// </summary>
        private void DrawFloorTiles(TileRect tr, ISpriteBatch spriteBatch)
            {
            DrawParameters drawParameters = default;
            drawParameters.AreaWithinTexture = Constants.TileRectangle;
            drawParameters.Centre = Vector2.Zero;

            for (int j = 0; j < tr.Height; j++)
                {
                int y = tr.TopLeft.Y + j;

                for (int i = 0; i < tr.Width; i++)
                    {
                    int x = tr.TopLeft.X + i;

                    string textureName = this._tiles[x, y].TextureName;
                    if (textureName == null)
                        continue;

                    var pathToTexture = textureName.Contains("/") ? textureName : "Tiles/" + textureName;
                    drawParameters.Texture = this._spriteLibrary.GetSprite(pathToTexture);
                    drawParameters.Position = new Vector2(x, y) * Constants.TileSize;
                    spriteBatch.DrawTexture(drawParameters);
                    }
                }
            }
        }
    }
