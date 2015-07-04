﻿using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Shot : MovingItem
        {
        protected Shot(World world, Vector2 position) : base(world, position)
            {
            }
        }
    }
