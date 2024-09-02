using System;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects
    {
    internal class BoundaryCurrentRoom : IBoundMovement
        {
        private readonly IMovingItem _gameObject;

        public BoundaryCurrentRoom(IMovingItem gameObject)
            {
            this._gameObject = gameObject ?? throw new ArgumentNullException(nameof(gameObject));
            }

        public bool IsPositionWithinBoundary(TilePos tilePos)
            {
            var roomBoundary = World.GetContainingRoom(this._gameObject.TilePosition);
            var result = roomBoundary.Contains(tilePos);
            return result;
            }
        }
    }
