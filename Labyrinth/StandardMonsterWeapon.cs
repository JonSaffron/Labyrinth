using System;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class StandardMonsterWeapon : IMonsterWeapon
        {
        public void FireIfYouLike(StaticItem source)
            {
            Direction firingDirection = DetermineFiringDirection(source.TilePosition, GlobalServices.GameState.Player.TilePosition);
            if (firingDirection == Direction.None)
                return;
            if (!DoesMonsterHaveClearShotAtPlayer(source.TilePosition, firingDirection))
                return;

            var startPos = source.TilePosition.ToPosition() + firingDirection.ToVector() * (Constants.CentreOfTile);
            GlobalServices.GameState.AddStandardShot(startPos, firingDirection, source.Energy >> 2, ShotType.Monster);
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

        private static bool DoesMonsterHaveClearShotAtPlayer(TilePos monsterTilePos, Direction firingDirection)
            {
            int i = 0;
            var testPos = monsterTilePos;
            var playerTilePos = GlobalServices.GameState.Player.TilePosition;
            for ( ; testPos != playerTilePos && i < 20; testPos = testPos.GetPositionAfterOneMove(firingDirection), i++)
                {
                bool isBlocked = GlobalServices.GameState.IsImpassableItemOnTile(testPos);
                if (isBlocked)
                    return false;
                }
            return true;
            }
        }
    }
