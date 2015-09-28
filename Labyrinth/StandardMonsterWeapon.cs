using System;
using Labyrinth.GameObjects;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class StandardMonsterWeapon : IMonsterWeapon
        {
        public void FireIfYouLike(StaticItem source, World world)
            {
            Direction firingDirection = DetermineFiringDirection(source.TilePosition, world.Player.TilePosition);
            if (firingDirection == Direction.None)
                return;
            if (!DoesMonsterHaveClearShotAtPlayer(world, source.TilePosition, firingDirection))
                return;

            var startPos = source.TilePosition.ToPosition() + firingDirection.ToVector() * new Vector2(Tile.Width, Tile.Height) / 2;
            var shot = new StandardShot(world, startPos, firingDirection, source.Energy >> 2, ShotType.Monster);
            world.AddShot(shot);
            source.PlaySound(GameSound.MonsterShoots);
            }

        private static Direction DetermineFiringDirection(TilePos fromTilePos, TilePos towardsTilePos)
            {
            int xDiff = Math.Sign(fromTilePos.X - towardsTilePos.X);
            int yDiff = Math.Sign(fromTilePos.Y - towardsTilePos.Y);
            if (xDiff == 0)
                {
                switch (yDiff)
                    {
                    case 1: return Direction.Up;
                    case -1: return Direction.Down;
                    }
                }
            else if (yDiff == 0)
                {
                switch (xDiff)
                    {
                    case 1: return Direction.Left;
                    case -1: return Direction.Right;
                    }
                }
            return Direction.None;
            }

        private static bool DoesMonsterHaveClearShotAtPlayer(World world, TilePos monsterTilePos, Direction firingDirection)
            {
            int i = 0;
            var testPos = monsterTilePos;
            var playerTilePos = world.Player.TilePosition;
            for ( ; testPos != playerTilePos && i < 20; testPos = testPos.GetPositionAfterOneMove(firingDirection), i++)
                {
                bool isClear = world.CanTileBeOccupied(testPos, false);
                if (!isClear)
                    return false;
                }
            return true;
            }
        }
    }
