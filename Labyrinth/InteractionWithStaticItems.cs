using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.GameObjects;
using Labyrinth.Services.Messages;

namespace Labyrinth
    {
    class InteractionWithStaticItems : IInteraction
        {
        private readonly IGameObject _staticItem;
        private readonly IMovingItem _movingItem;

        public InteractionWithStaticItems(IGameObject staticItem, IMovingItem movingItem)
            {
            if (staticItem == null)
                throw new ArgumentNullException(nameof(staticItem));

            if (staticItem is IMovingItem)
                throw new ArgumentOutOfRangeException(nameof(staticItem));

            this._staticItem = staticItem;

            this._movingItem = movingItem ?? throw new ArgumentNullException(nameof(movingItem));
            }

        public void Collide()
            {
            if (!this._movingItem.IsExtant || !this._staticItem.IsExtant)
                return;

            if (this._staticItem.Properties.Get(GameObjectProperties.DeadlyToTouch) && (this._movingItem != null || this._movingItem is IMonster))
                {
                this._movingItem.InstantlyExpire();
                return;
                }

            if (this._movingItem is Player player)
                {
                InteractionInvolvesPlayer(player, this._staticItem);
                return;
                }

            if (this._movingItem is IMunition munition)
                {
                InteractionInvolvesMunition(munition, this._staticItem);
                // ReSharper disable once RedundantJumpStatement
                return;
                }

            }

        private static void InteractionInvolvesPlayer(Player player, IGameObject staticItem)
            {
            if (staticItem is Crystal crystal)
                {
                PlayerTakesCrystal(player, crystal);
                return;
                }

            if (staticItem is Fruit fruit)
                {
                PlayerEatsFruit(player, fruit);
                return;
                }

            if (staticItem is Mushroom mushroom)
                {
                PlayerPoisonedByMushroom(mushroom, player);
                // return;
                }
            }

        private static void PlayerPoisonedByMushroom(Mushroom mushroom, Player player)
            {
            player.PlaySound(GameSound.PlayerInjured);
            int r = mushroom.CalculateEnergyToRemove(player);
            if (r > 0)
                player.ReduceEnergy(r);
            mushroom.SetTaken();
            }

        private static void PlayerEatsFruit(Player player, Fruit fruit)
            {
            player.PlaySound(GameSound.PlayerEatsFruit);
            player.AddEnergy(fruit.Energy);
            fruit.SetTaken();
            }

        private static void PlayerTakesCrystal(Player player, Crystal crystal)
            {
            player.AddEnergy(crystal.Energy);
            var crystalTaken = new CrystalTaken(crystal);
            Messenger.Default.Send(crystalTaken);
            player.CrystalCollected(crystal);
            crystal.SetTaken();
            bool doAnyCrystalsRemain = GlobalServices.GameState.DistinctItemsOfType<Crystal>().Any(c => c.IsExtant);
            if (doAnyCrystalsRemain)
                {
                GlobalServices.SoundPlayer.Play(GameSound.PlayerCollectsCrystal);
                }
            else
                {
                Messenger.Default.Send(new WorldCompleted());
                }
            }

        private static void InteractionInvolvesMunition(IMunition munition, IGameObject staticItem)
            {
            EffectOfShot effectOfShot = staticItem.Properties.Get(GameObjectProperties.EffectOfShot);

            if (effectOfShot == EffectOfShot.Impervious)
                {
                munition.InstantlyExpire();
                return;
                }

            if (effectOfShot == EffectOfShot.Injury)
                {
                int energyOfStaticItem = staticItem.Energy;
                staticItem.ReduceEnergy(munition.Energy);

                if (staticItem.IsExtant)
                    {
                    staticItem.PlaySound(GameSound.StaticObjectShotAndInjured);
                    GlobalServices.GameState.ConvertShotToBang(munition);
                    }
                else
                    {
                    var bang = GlobalServices.GameState.AddBang(staticItem.Position, BangType.Short);
                    bang.PlaySound(GameSound.StaticObjectShotAndInjured);
                    if (munition is StandardShot)
                        {
                        munition.InstantlyExpire();
                        }
                    else
                        {
                        munition.ReduceEnergy(energyOfStaticItem);
                        }
                    }

                return;
                }

            if (effectOfShot == EffectOfShot.Reflection && munition is IStandardShot standardShot && !standardShot.HasRebounded)
                {
                standardShot.Reverse();
                }

            }
        }
    }
