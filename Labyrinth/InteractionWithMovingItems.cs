using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class InteractionWithMovingItems : IInteraction
        {
        private readonly MovingItem _movingItem1;
        private readonly MovingItem _movingItem2;

        public InteractionWithMovingItems([NotNull] MovingItem movingItem1, [NotNull] MovingItem movingItem2)
            {
            this._movingItem1 = movingItem1 ?? throw new ArgumentNullException(nameof(movingItem1));
            this._movingItem2 = movingItem2 ?? throw new ArgumentNullException(nameof(movingItem2));
            }

        public void Collide()
            {
            if (!this._movingItem1.IsExtant || !this._movingItem2.IsExtant)
                return;

            var items = new Collection<MovingItem> { this._movingItem1, this._movingItem2 };

            var explosion = items.OfType<Explosion>().FirstOrDefault();
            if (explosion != null)
                {
                var otherItem = items.Single(item => item != explosion);
                InteractionInvolvesExplosion(explosion, otherItem);
                return;
                }
                
            var shot = items.OfType<StandardShot>().FirstOrDefault();
            if (shot != null)
                {
                var otherItem = items.Single(item => item != shot);
                if (InteractionInvolvingShot(shot, otherItem))
                    return;
                }

            var mine = items.OfType<Mine>().FirstOrDefault();
            if (mine != null)
                {
                var otherItem = items.Single(item => item != mine);
                mine.SteppedOnBy(otherItem);
                return;
                }

            var player = items.OfType<Player>().SingleOrDefault();
            if (player != null)
                {
                var monster = items.Single(item => item != player) as Monster;
                if (monster != null)
                    {
                    int monsterEnergy = monster.InstantlyExpire();
                    player.ReduceEnergy(monsterEnergy);
                    GlobalServices.GameState.AddBang(monster.Position, BangType.Long, GameSound.PlayerCollidesWithMonster);
                    return;
                    }
                }
            
            var moveableObject = items.FirstOrDefault(item => item.Solidity == ObjectSolidity.Moveable);
            if (moveableObject != null)
                {
                var otherObject = items.Single(item => item != moveableObject);
                if (otherObject.Capability.CanMoveAnother())
                    {
                    var actionTaken = PushOrBounceObject(moveableObject, otherObject);
                    if (actionTaken)
                        return;
                    }

                if (CrushObject(moveableObject, otherObject))
                    // ReSharper disable once RedundantJumpStatement
                    return;
                }

            // any other interaction here...
            }

        /// <summary>
        /// Deals with the situation where one object could be moving another
        /// </summary>
        /// <param name="moveableObject">An item such as the boulder</param>
        /// <param name="movingObject">An item that is capable of moving another item such as the player or a shot</param>
        /// <returns></returns>
        private static bool PushOrBounceObject([NotNull] MovingItem moveableObject, [NotNull] MovingItem movingObject)
            {
            var result = ShouldStartPushOrBounce(moveableObject, movingObject);
            if (result)
                {
                moveableObject.PushOrBounce(movingObject, movingObject.CurrentMovement.Direction);
                if (movingObject is StandardShot standardShot)
                    GlobalServices.GameState.ConvertShotToBang(standardShot);
                }
            return result;            
            }

        private static bool ShouldStartPushOrBounce([NotNull] MovingItem moveableObject, [NotNull] MovingItem movingObject)
            {
            bool isMoveableObjectAlreadyInMotion = moveableObject.CurrentMovement.IsMoving;
            bool isMovingObjectMotionless = !movingObject.CurrentMovement.IsMoving;
            if (isMoveableObjectAlreadyInMotion || isMovingObjectMotionless) 
                return false;

            float currentDistanceApart = Vector2.Distance(movingObject.Position, moveableObject.Position);
            if (!(currentDistanceApart < 40)) 
                return false;

            Vector2 positionWithMovement = movingObject.Position + movingObject.CurrentMovement.Direction.ToVector();
            float potentialDistanceApart = Vector2.Distance(positionWithMovement, moveableObject.Position);
            bool isGettingCloser = (potentialDistanceApart < currentDistanceApart);
            return isGettingCloser;
            }

        private static bool CrushObject([NotNull] MovingItem moveableObject, [NotNull] MovingItem movingObject)
            {
            if (IsCrushingPossible(moveableObject, movingObject))
                {
                var movingObjectPosition = movingObject.CurrentMovement.Direction == Direction.None ? movingObject.Position : movingObject.CurrentMovement.MovingTowards;
                var movingObjectTile = TilePos.TilePosFromPosition(movingObjectPosition);
                var moveableObjectTile = TilePos.TilePosFromPosition(moveableObject.CurrentMovement.MovingTowards);
                if (movingObjectTile == moveableObjectTile)
                    {
                    var energy = movingObject.InstantlyExpire();
                    var b = GlobalServices.GameState.AddBang(movingObject.Position, BangType.Long);
                    if (movingObject is Monster monster)
                        {
                        b.PlaySound(GameSound.MonsterDies);
                        GlobalServices.ScoreKeeper.EnemyCrushed(monster, energy);
                        }
                    return true;
                    }
                }
            return false;
            }

        private static bool IsCrushingPossible([NotNull] MovingItem moveableObject, [NotNull] MovingItem movingObject)
            {
            if (!moveableObject.CurrentMovement.IsMoving)
                return false;
            if (movingObject.Capability.CanMoveAnother())
                {
                var result = moveableObject.CurrentMovement.Direction != movingObject.CurrentMovement.Direction;
                return result;
                }
            return true;
            }

        private static void InteractionInvolvesExplosion(Explosion explosion, MovingItem movingItem)
            {
            if (movingItem is Shot shot)
                {
                shot.ReduceEnergy(explosion.Energy);
                explosion.InstantlyExpire();
                }

            if (movingItem is Monster monster)
                {
                var energyRemoved = Math.Min(explosion.Energy, monster.Energy);
                GlobalServices.ScoreKeeper.EnemyShot(monster, energyRemoved);
                monster.ReduceEnergy(explosion.Energy);
                explosion.InstantlyExpire();
                return;
                }

            if (movingItem is Player player)
                {
                var explosionEnergy = explosion.Energy;
                explosion.InstantlyExpire();
                player.ReduceEnergy(explosionEnergy);
                if (movingItem.IsAlive())
                    player.PlaySound(GameSound.PlayerInjured);
                // ReSharper disable once RedundantJumpStatement
                return;
                }

            // any other interaction here...
            }

        private static bool InteractionInvolvingShot(StandardShot shot, MovingItem movingItem)
            {
            if (movingItem is Player && (shot.ShotType == ShotType.Monster || (shot.ShotType == ShotType.Player && shot.HasRebounded)))
                {
                var shotEnergy = shot.Energy;
                GlobalServices.GameState.ConvertShotToBang(shot);
                movingItem.ReduceEnergy(shotEnergy);
                if (movingItem.IsAlive())
                    movingItem.PlaySound(GameSound.PlayerInjured);
                return true;
                }

            if (movingItem is Monster monster)
                {
                var result = ShotHitsMonster(shot, monster);
                return result;
                }

            if (movingItem is StandardShot standardShot2)
                {
                var result = ShotHitsShot(shot, standardShot2);
                return result;
                }

            return false;
            }

        private static bool ShotHitsShot(StandardShot standardShot1, StandardShot standardShot2)
            {
            if (standardShot2.ShotType == standardShot1.ShotType ||
                standardShot2.DirectionOfTravel != standardShot1.DirectionOfTravel.Reversed())
                {
                return false;
                }
            
            // todo: check what the original game does. It may not create more than one bang.
            int minEnergy = Math.Min(standardShot2.Energy, standardShot1.Energy);
            Bang bang = null;
            standardShot2.ReduceEnergy(minEnergy);
            if (!standardShot2.IsExtant)
                bang = GlobalServices.GameState.ConvertShotToBang(standardShot2);
            standardShot1.ReduceEnergy(minEnergy);
            if (!standardShot1.IsExtant)
                bang = GlobalServices.GameState.ConvertShotToBang(standardShot1);
            bang?.PlaySound(GameSound.StaticObjectShotAndInjured);
            return true;
            }

        private static bool ShotHitsMonster(StandardShot shot, Monster monster)
            {
            if (monster.ShotsBounceOff)
                {
                if (!shot.HasRebounded)
                    shot.Reverse();
                return false;
                }

            var energyRemoved = Math.Min(shot.Energy, monster.Energy);
            GlobalServices.ScoreKeeper.EnemyShot(monster, energyRemoved);
            monster.ReduceEnergy(shot.Energy);
            GlobalServices.GameState.ConvertShotToBang(shot);
            return true;
            }
        }
    }
