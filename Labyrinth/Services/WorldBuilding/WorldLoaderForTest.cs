﻿using System;
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

        private int Height 
            {
            get
                {
                var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
                var result = lines.GetLength(0);
                return result;
                }
            }

        private int Width 
            {
            get
                {
                var lines = this._layout.Split(new [] { "\r\n" }, StringSplitOptions.None);
                var result = lines.Max(line => line.Length);
                return result;
                }
            }

        public TilePos WorldSize => new TilePos(this.Width, this.Height);

        public Tile[,] GetFloorTiles()
            {
            var result = new Tile[this.Width, this.Height];
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                    result[x, y] = new Tile(null, 0);
            return result;
            }

        public bool RestartInSameRoom => false;

        public Dictionary<int, PlayerStartState> GetPlayerStartStates()
            {
            var result = new Dictionary<int, PlayerStartState>();
            result.Add(0, new PlayerStartState(new TilePos(), 100));
            return result;
            }

        public void AddGameObjects(GameState gameState)
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
                        case 'm':
                            var mdef = new MonsterDef();
                            mdef.Type = typeof(DummyMonster);

                            gameState.AddMonster(mdef);
                        default:
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
        }
    }
