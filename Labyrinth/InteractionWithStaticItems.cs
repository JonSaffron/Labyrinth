﻿using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.GameObjects;
using Labyrinth.Services.Messages;

namespace Labyrinth
    {
    class InteractionWithStaticItems : IInteraction
        {
        private readonly World _world;
        private readonly IGameObject _staticItem;
        private readonly IMovingItem _movingItem;

        public InteractionWithStaticItems(World world, IGameObject staticItem, IMovingItem movingItem)
            {
            this._world = world ?? throw new ArgumentNullException(nameof(world));

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

            if (this._movingItem is Player player)
                {
                InteractionInvolvesPlayer(this._world, player, this._staticItem);
                return;
                }

            if (this._movingItem is IMunition munition)
                {
                InteractionInvolvesMunition(munition, this._staticItem);
                // return;
                }
            }

        private static void InteractionInvolvesPlayer(World world, Player player, IGameObject staticItem)
            {
            if (staticItem is Crystal crystal)
                {
                PlayerTakesCrystal(world, player, crystal);
                return;
                }

            if (staticItem is ForceField)
                {
                player.InstantlyExpire();
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

        private static void PlayerTakesCrystal(World world, Player player, Crystal crystal)
            {
            // todo this didn't seem to end the world - test that checking IsExtant fixes the problem
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
                foreach (IMonster monster in GlobalServices.GameState.DistinctItemsOfType<IMonster>())
                    {
                    monster.InstantlyExpire();
                    }

                // todo some graphical effect - original game changed colours
                GlobalServices.SoundPlayer.PlayWithCallback(GameSound.PlayerFinishesWorld,
                    (sender, args) => world.SetLevelReturnType(LevelReturnType.FinishedWorld));
                world.SetDoNotUpdate();
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
