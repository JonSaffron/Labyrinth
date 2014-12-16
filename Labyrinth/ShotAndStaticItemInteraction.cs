using System;

namespace Labyrinth
    {
    class ShotAndStaticItemInteraction : IInteraction
        {
        private readonly World _world;
        private readonly Shot _shot;
        private readonly StaticItem _staticItem;

        public ShotAndStaticItemInteraction(World world, Shot shot, StaticItem staticItem)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (shot == null)
                throw new ArgumentNullException("shot");
            if (staticItem == null)
                throw new ArgumentNullException("staticItem");
            if (staticItem is MovingItem)
                throw new ArgumentOutOfRangeException("staticItem");

            this._world = world;
            this._shot = shot;
            this._staticItem = staticItem;
            }

        public void Collide()
            {
            if (!this._shot.IsExtant || !this._staticItem.IsExtant)
                return;

            if (this._staticItem is ForceField)
                {
                if (!this._shot.HasRebounded)
                    this._shot.Reverse();
                return;
                }

            if (this._staticItem is Wall)
                {
                this._shot.InstantlyExpire();
                return;
                }

            if (!(this._staticItem is Fruit) && !(this._staticItem is Grave) && !(this._staticItem is Mushroom)) 
                return;

            this._world.Game.SoundLibrary.Play(GameSound.StaticObjectShotAndInjured);
            this._staticItem.ReduceEnergy(this._shot.Energy);
            if (this._staticItem.IsExtant)
                this._world.ConvertShotToBang(this._shot);
            else
                this._world.AddShortBang(this._staticItem.Position);
            this._shot.InstantlyExpire();
            }
        }
    }
