using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.PathFinder;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public void RenderWorld(TileRect viewOfWorld, [NotNull] ISpriteBatch spriteBatch)
            {
            if (spriteBatch == null) 
                throw new ArgumentNullException(nameof(spriteBatch));

            DrawFloorTiles(spriteBatch, viewOfWorld);

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
                var renderAnimation = item.RenderAnimation;

                renderAnimation?.Draw(spriteBatch, this._spriteLibrary);
                }
            }

        /// <summary>
        /// Draws the floor of the current view
        /// </summary>
        private void DrawFloorTiles(ISpriteBatch spriteBatch, TileRect tr)
            {
            for (int j = 0; j < tr.Height; j++)
                {
                int y = tr.TopLeft.Y + j;

                for (int i = 0; i < tr.Width; i++)
                    {
                    int x = tr.TopLeft.X + i;

                    Texture2D texture = this._tiles[x, y].Floor;
                    if (texture != null)
                        {
                        Vector2 position = new Vector2(x, y) * Constants.TileSize;
                        spriteBatch.DrawEntireTextureInWindow(texture, position);
                        }
                    }
                }
            }

        }
    }
