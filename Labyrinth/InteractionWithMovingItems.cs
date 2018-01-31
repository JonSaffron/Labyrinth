using System;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Labyrinth.GameObjects;
using Labyrinth.Services.Messages;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class InteractionWithMovingItems : IInteraction
        {
        /// <summary>
        /// This game object will have just moved its position (or at least returned true from its update method)
        /// </summary>
        private readonly MovingItem _primaryItem;

        /// <summary>
        /// A second game object that is touching the first
        /// </summary>
        private readonly MovingItem _secondaryItem;

        public InteractionWithMovingItems([NotNull] MovingItem primaryItem, [NotNull] MovingItem secondaryItem)
            {
            this._primaryItem = primaryItem ?? throw new ArgumentNullException(nameof(primaryItem));
            this._secondaryItem = secondaryItem ?? throw new ArgumentNullException(nameof(secondaryItem));
            }

        public void Collide()
            {
            if (!this._primaryItem.IsExtant || !this._secondaryItem.IsExtant)
                return;

            if (this._primaryItem is Explosion explosion)
                {
                if (InteractionInvolvesExplosion(explosion, this._secondaryItem))
                    return;
                }

            if (this._primaryItem is StandardShot shot)
                {
                if (InteractionInvolvingShot(shot, this._secondaryItem))
                    return;
                }

            if (this._primaryItem is Mine mine)
                {
                mine.SteppedOnBy(this._secondaryItem);
                return;
                }

            if (this._primaryItem is Player player && this._secondaryItem is Monster monster)
                {
                PlayerAndMonsterCollide(player, monster);
                }
            else
                {
                player = this._secondaryItem as Player;
                monster = this._primaryItem as Monster;
                if (player != null && monster != null)
                    {
                    PlayerAndMonsterCollide(player, monster);
                    }
                }

            var moveableObject = this._secondaryItem.Solidity == ObjectSolidity.Moveable ? this._secondaryItem : null;
            if (moveableObject != null)
                {
                var otherObject = this._primaryItem;
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

        private static void PlayerAndMonsterCollide(Player player, Monster monster)
            {
            int monsterEnergy = monster.Energy;
            monster.InstantlyExpire();
            player.ReduceEnergy(monsterEnergy);
            GlobalServices.GameState.AddBang(monster.Position, BangType.Long, GameSound.PlayerCollidesWithMonster);
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
                    var b = GlobalServices.GameState.AddBang(movingObject.Position, BangType.Long);
                    if (movingObject is Monster monster)
                        {
                        b.PlaySound(GameSound.MonsterDies);
                        var monsterCrushed = new MonsterCrushed(monster, moveableObject);
                        Messenger.Default.Send(monsterCrushed);
                        }
                    movingObject.InstantlyExpire();
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
                var isMovingInDifferentDirection = moveableObject.CurrentMovement.Direction != movingObject.CurrentMovement.Direction;
                return isMovingInDifferentDirection;
                }
            var result = movingObject.Solidity == ObjectSolidity.Insubstantial;
            return result;
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
                var monsterShot = new MonsterShot(monster, movingItem);
                Messenger.Default.Send(monsterShot);
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

            var monsterKilledByShot = new MonsterShot(monster, shot);
            Messenger.Default.Send(monsterKilledByShot);

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
