using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Labyrinth.Services.WorldBuilding
    {
    class WorldLoaderForTest : IWorldLoader
        {
        private string _layout;

        public void LoadWorld(string worldLayout)
            {
            this._layout = worldLayout;
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

        public Tile[,] GetFloorTiles()
            {
            throw new NotImplementedException();
            }

        public Dictionary<int, StartState> GetStartStates()
            {
            throw new NotImplementedException();
            }

        public void GetGameObjects(GameState gameState)
            {
            var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
            for (int y = 0; y < lines.Count(); y++)
                {
                for (int x = 0; x < lines[y].Length; x++)
                    {
                    var tp = new TilePos(x, y);
                    var position = tp.ToPosition();
                    var c = lines[y][x];
                    switch (c)
                        {
                        case ' ':
                            break;
                        case '#':
                            Trace.WriteLine(System.IO.Directory.GetCurrentDirectory());
                            gameState.AddWall(position, "Tiles/Floor1");
                            break;
                        case 'p':
                            gameState.AddPlayer(position, 255, 0);
                            break;
                        case 'b':
                            gameState.AddBoulder(position);
                            break;
                        case 'g':
                            gameState.AddGrave(tp);
                            break;
                        default:
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
        }
    }
