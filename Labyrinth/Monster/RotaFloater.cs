using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    abstract class RotaFloater : Monster
        {
        protected int ChanceOfAggressiveMove;

        protected RotaFloater(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return MonsterMovement.DetermineDirectionRolling;
                case MonsterMobility.Patrolling:
                    return MonsterMovement.DetermineDirectionStandardPatrolling;
                case MonsterMobility.Aggressive:
                    Func<Monster, Direction> result = m => DetermineDirectionAggressive(this, ShouldMakeAnAggressiveMove);
                    return result;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        protected override Monster Clone()
            {
            var result = (RotaFloater)this.MemberwiseClone();
            return result;
            }

        private static Direction DetermineDirectionAggressive(Monster m, Func<bool> shouldMakeAnAggressiveMove)
            {
            var p = m.World.Player;
            if (p == null || !p.IsExtant)
                {
                return MonsterMovement.DetermineDirectionRolling(m);
                }
            
            TilePos tp = TilePos.TilePosFromPosition(m.Position);
/*            bool isCurrentlyMovingTowardsFreeSpace;
            if (this.Direction == Direction.None) 
                isCurrentlyMovingTowardsFreeSpace = false;
            else
                {
                TilePos pp = TilePos.GetPositionAfterOneMove(tp, this.Direction);
                isCurrentlyMovingTowardsFreeSpace = this._world.IsTileUnoccupied(pp, true);
                
                if (isCurrentlyMovingTowardsFreeSpace)
                    {
                    Vector2 newPos = World.GetBounds(pp).GetCentre();
                    if (!IsInSameRoom(this.Position, newPos) && MonsterRandom.Next(4) != 0)
                        isCurrentlyMovingTowardsFreeSpace = false;
                    }
                }
*/            
            Direction newDirection = Direction.None;
            TilePos playerPosition = TilePos.TilePosFromPosition(p.Position);
            int yDiff = tp.Y - playerPosition.Y;
            int xDiff = tp.X - playerPosition.X;
            if (m.Direction == Direction.Left || m.Direction == Direction.Right)
                {
                if (Math.Abs(tp.X - playerPosition.X) == 1 && Math.Abs(yDiff) > 3 && Math.Abs(yDiff) <= 16 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                    }
                else if ((tp.X - playerPosition.X) == 0 && Math.Abs(yDiff) <= 3 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                    }
                }
            else if (m.Direction == Direction.Up || m.Direction == Direction.Down)
                {
                if (Math.Abs(tp.Y - playerPosition.Y) == 1 && Math.Abs(xDiff) > 3 && Math.Abs(xDiff) <= 16 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                    }
                else if ((tp.Y - playerPosition.Y) == 0 && Math.Abs(xDiff) <= 3 && shouldMakeAnAggressiveMove())
                    {
                    newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                    }
                }
            if (newDirection != Direction.None)
                {
                TilePos pp = TilePos.GetPositionAfterOneMove(tp, newDirection);
                if (m.World.CanTileBeOccupied(pp, false))
                    {
                    return newDirection;
                    }
                }

            return MonsterMovement.DetermineDirectionRolling(m);
            }

        private bool ShouldMakeAnAggressiveMove()
            {
            return MonsterRandom.Next(this.ChanceOfAggressiveMove) == 0; 
            }
        
        }
    }
