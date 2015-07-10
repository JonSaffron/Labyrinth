using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.WorldBuilding
    {
    class WorldLoaderForTest : IWorldLoader
        {
        private string _layout;

        public void LoadWorld(string worldLayout)
            {
            this._layout = worldLayout;
            }

        public int GetMaximumWorldAreaId()
            {
            return 0;
            }

        public StartState GetStartStateForWorldAreaId(int id)
            {
            var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
            TilePos? tp = null;
            for (int y = 0; y < lines.Count(); y++)
                {
                int x = lines[y].IndexOf('p');
                if (x != -1)
                    {
                    tp = new TilePos(x, y); 
                    break;
                    }
                }
            if (!tp.HasValue)
                throw new InvalidOperationException();
            var result = new StartState(tp.Value, 255);
            return result;
            }

        public int GetWorldAreaIdForTilePos(TilePos tp)
            {
            return 0;
            }

        public int Height 
            {
            get
                {
                var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
                var result = lines.GetLength(0);
                return result;
                }
            }

        public int Width 
            {
            get
                {
                var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
                var result = lines.Max(line => line.Length);
                return result;
                }
            }

        public Tile this[TilePos tp]
            {
            get
                {
                return null;
                }
            }

        public Texture2D LoadTexture(ContentManager contentManager, string textureName)
            {
            return null;
            }

        public IEnumerable<StaticItem> GetGameObjects(World world)
            {
            var result = new List<StaticItem>();
            var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
            for (int y = 0; y < lines.Count(); y++)
                {
                for (int x = 0; x < lines[y].Length; x++)
                    {
                    var position = new TilePos(x, y).ToPosition();
                    var c = lines[y][x];
                    switch (c)
                        {
                        case ' ':
                            break;
                        case '#':
                            Trace.WriteLine(System.IO.Directory.GetCurrentDirectory());
                            result.Add(new Wall(world, position, "Tiles/Floor1"));
                            break;
                        case 'p':
                            result.Add(new Player(world, position, 255));
                            break;
                        case 'b':
                            result.Add(new Boulder(world, position));
                            break;
                        case 'g':
                            result.Add(new Grave(world, position));
                            break;
                        default:
                            throw new InvalidOperationException();
                        }
                    }
                }
            return result;
            }
        }
    }
