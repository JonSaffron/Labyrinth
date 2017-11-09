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
                if (InteractionInvolvesExplosion(explosion, otherItem))
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

        private static bool InteractionInvolvesExplosion(Explosion explosion, MovingItem movingItem)
            {
            if (movingItem is Shot shot)
                {
                shot.ReduceEnergy(explosion.Energy);
                return true;
                }

            if (movingItem is Monster monster)
                {
                var energyRemoved = Math.Min(explosion.Energy, monster.Energy);
                GlobalServices.ScoreKeeper.EnemyShot(monster, energyRemoved);
                monster.ReduceEnergy(explosion.Energy);
                return true;
                }

            if (movingItem is Player player)
                {
                var explosionEnergy = explosion.Energy;
                player.ReduceEnergy(explosionEnergy);
                if (movingItem.IsAlive())
                    player.PlaySound(GameSound.PlayerInjured);
                return true;
                }

            return false;
            }

        private static bool InteractionInvolvingShot(StandardShot shot, MovingItem movingItem)
            {
            if ((movingItem is Player || movingItem is Monster) && (shot.Originator != movingItem || shot.HasRebounded))
                {
                var result = ShotHitsPlayerOrMonster(shot, movingItem);
                return result;
                }

            if (movingItem is StandardShot standardShot2)
                {
                var result = ShotHitsShot(shot, standardShot2);
                return result;
                }

            return false;
            }

        private static bool ShotHitsPlayerOrMonster(StandardShot shot, MovingItem playerOrMonster)
            {
            Monster monster = playerOrMonster as Monster;
            if (monster != null && monster.ShotsBounceOff)
                {
                // A rebounded shot cannot hurt a monster that shots rebound off but it doesn't rebound again
                if (!shot.HasRebounded)
                    shot.Reverse();
                return false;
                }

            // no score from a rebound or from an enemy shot
            if (!shot.HasRebounded && shot.Originator is Player && monster != null)
                {
                var energyRemovedForScoringPurposes = Math.Min(shot.Energy, monster.Energy);
                GlobalServices.ScoreKeeper.EnemyShot(monster, energyRemovedForScoringPurposes);
                }

            playerOrMonster.ReduceEnergy(shot.Energy);
            GlobalServices.GameState.ConvertShotToBang(shot);

            if (playerOrMonster.IsAlive())
                {
                if (playerOrMonster is Player)
                    playerOrMonster.PlaySound(GameSound.PlayerInjured);
                else if (monster != null)
                    {
                    var gs = monster.IsEgg ? GameSound.PlayerShootsAndInjuresEgg : GameSound.PlayerShootsAndInjuresMonster;
                    monster.PlaySound(gs);
                    }
                }

            return true;
            }

        private static bool ShotHitsShot(StandardShot standardShot1, StandardShot standardShot2)
            {
            // this is the same logic the original game uses
            var isAtLeastOneShotFromMonsterWhichHasNotRebounded = (standardShot1.Originator is Monster && !standardShot1.HasRebounded) || (standardShot2.Originator is Monster && !standardShot2.HasRebounded);
            if (!isAtLeastOneShotFromMonsterWhichHasNotRebounded)
                {
                return false;
                }

            // this is new to this adaptation
            if (standardShot2.Orientation != standardShot1.Orientation)
                {
                return false;
                }
            
            int minEnergy = Math.Min(standardShot2.Energy, standardShot1.Energy);
            Bang bang = null;
            standardShot2.ReduceEnergy(minEnergy);
            if (!standardShot2.IsExtant)
                bang = GlobalServices.GameState.ConvertShotToBang(standardShot2);
            standardShot1.ReduceEnergy(minEnergy);
            if (!standardShot1.IsExtant && bang == null)    // only one bang needed from collision
                bang = GlobalServices.GameState.ConvertShotToBang(standardShot1);
            bang?.PlaySound(GameSound.StaticObjectShotAndInjured);
            return true;
            }
        }
    }
