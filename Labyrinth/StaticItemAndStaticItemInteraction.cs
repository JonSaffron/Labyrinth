using System;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class StaticItemAndStaticItemInteraction : IInteraction
        {
        private readonly World _world;
        private readonly StaticItem _staticItem1;
        private readonly StaticItem _staticItem2;

        public StaticItemAndStaticItemInteraction(World world, StaticItem staticItem1, StaticItem staticItem2)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (staticItem1 == null)
                throw new ArgumentNullException("staticItem1");
            if (staticItem2 == null)
                throw new ArgumentNullException("staticItem2");
            if (staticItem1 is MovingItem)
                throw new ArgumentOutOfRangeException("staticItem1");
            if (staticItem2 is MovingItem)
                throw new ArgumentOutOfRangeException("staticItem2");

            this._world = world;
            this._staticItem1 = staticItem1;
            this._staticItem2 = staticItem2;
            }

        public void Collide()
            {
            // nothing to do
            }
        }
    }
