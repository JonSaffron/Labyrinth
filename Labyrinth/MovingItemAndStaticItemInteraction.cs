using System;
using System.Linq;

namespace Labyrinth
{
    class MovingItemAndStaticItemInteraction : IInteraction
    {
        private readonly World _world;
        private readonly MovingItem _movingItem;
        private readonly StaticItem _staticItem;

        public MovingItemAndStaticItemInteraction(World world, MovingItem movingItem, StaticItem staticItem)
        {
            if (world == null)
                throw new ArgumentNullException("world");
            if (movingItem == null)
                throw new ArgumentNullException("movingItem");
            if (staticItem == null)
                throw new ArgumentNullException("staticItem");

            if (movingItem is Shot)
                throw new ArgumentOutOfRangeException("movingItem");
            if (staticItem is MovingItem)
                throw new ArgumentOutOfRangeException("staticItem");

            this._world = world;
            this._movingItem = movingItem;
            this._staticItem = staticItem;
        }

        public void Collide()
        {
            if (!this._movingItem.IsExtant || !this._staticItem.IsExtant)
                return;

            var player = this._movingItem as Player;
            if (player != null)
            {
                var crystal = this._staticItem as Crystal;
                if (crystal != null)
                {
                    player.AddEnergy(crystal.Energy);
                    this._world.IncreaseScore(crystal.Score);
                    player.CrystalCollected(crystal);
                    crystal.SetTaken();
                    int howManyCrystalsRemain = this._world.GameObjects.DistinctItemsOfType<Crystal>().Count();
                    if (howManyCrystalsRemain == 0)
                        {
                        this._world.Game.SoundPlayer.Play(GameSound.PlayerFinishesWorld, SoundEffectFinished);
                        this._world.SetDoNotUpdate();
                        }
                    else
                        this._world.Game.SoundPlayer.Play(GameSound.PlayerCollectsCrystal);
                    return;
                }

                var forceField = this._staticItem as ForceField;
                if (forceField != null)
                {
                    player.InstantlyExpire();
                    return;
                }

                var fruit = this._staticItem as Fruit;
                if (fruit != null)
                {
                    this._world.Game.SoundPlayer.Play(GameSound.PlayerEatsFruit);
                    player.AddEnergy(fruit.Energy);
                    fruit.SetTaken();
                    return;
                }

                var mushroom = this._staticItem as Mushroom;
                if (mushroom != null)
                {
                    this._world.Game.SoundPlayer.Play(GameSound.PlayerInjured);
                    int r = mushroom.CalculateEnergyToRemove(player);
                    if (r > 0)
                        player.ReduceEnergy(r);
                    mushroom.SetTaken();
                    return;
                }
            }

            // player/monster/block vs Bang/Crystal/ForceField/Fruit/Grave/Mushroom/Tile
            // player - crystal, forcefield, fruit, mushroom

        }

        private void SoundEffectFinished(object sender, SoundEffectFinishedEventArgs args)
            {
            this._world.SetLevelReturnType(LevelReturnType.FinishedLevel);
            }
    }
}
