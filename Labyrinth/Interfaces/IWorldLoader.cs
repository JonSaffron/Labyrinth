using System.Collections.Generic;
using Labyrinth.GameObjects;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface IWorldLoader
        {
        void LoadWorld(string levelName);

        int GetMaximumWorldAreaId();
        StartState GetStartStateForWorldAreaId(int id);
        int GetWorldAreaIdForTilePos(TilePos tp);

        int Height { get; }
        int Width { get; }
        Tile this[TilePos tp] { get; }
        Texture2D LoadTexture(ContentManager contentManager, string textureName);
        IEnumerable<StaticItem> GetGameObjects(World world);
        }
    }
