﻿using System;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects
    {
    public static class GameObjectProperties
        {
        /// <summary>
        /// Determines the order that objects are drawn in
        /// </summary>
        /// <remarks>Objects with a low DrawOrder are drawn before those with a higher DrawOrder.
        /// Objects with the same DrawOrder may be drawn in any order.</remarks>
        public static readonly PropertyDef<SpriteDrawOrder> DrawOrder = new PropertyDef<SpriteDrawOrder>(nameof(DrawOrder), 0);

        /// <summary>
        /// An indication of how solid the object is
        /// </summary>
        /// <remarks>This is used to determine whether objects can share space and how they may interact</remarks>
        public static readonly PropertyDef<ObjectSolidity> Solidity = new PropertyDef<ObjectSolidity>(nameof(Solidity), ObjectSolidity.Stationary);

        /// <summary>
        /// An indication of what an object is capable of doing to another object
        /// </summary>
        public static readonly PropertyDef<ObjectCapability> Capability = new PropertyDef<ObjectCapability>(nameof(Capability), ObjectCapability.CannotMoveOthers);

        /// <summary>
        /// An indication of what happens when an object is shot
        /// </summary>
        public static readonly PropertyDef<EffectOfShot> EffectOfShot = new PropertyDef<EffectOfShot>(nameof(EffectOfShot), Labyrinth.EffectOfShot.Injury);

        /// <summary>
        /// Indicates whether the player scores for killing the monster
        /// </summary>
        public static readonly PropertyDef<bool> MonsterScoresWhenKilled = new PropertyDef<bool>(nameof(MonsterScoresWhenKilled), false);

        /// <summary>
        /// Indicates whether the object instantly kills the player when touched
        /// </summary>
        public static readonly PropertyDef<bool> DeadlyToTouch = new PropertyDef<bool>(nameof(DeadlyToTouch), false);

        // todo
        //// <summary>
        //// Defines how an object reduces the player's energy
        //// </summary>
        //public static readonly PropertyDef<Func<Player, int>> InjuriousToPlayer = new PropertyDef<Func<Player, int>>(nameof(InjuriousToPlayer), player => 0);

        // todo
        //// <summary>
        //// Defines how an object increases the player's energy
        //// </summary>
        //public static readonly PropertyDef<Func<Player, int>> CurativeToPlayer = new PropertyDef<Func<Player, int>>(nameof(CurativeToPlayer), player => 0);

        /// <summary>
        /// Defines an object's movement strategy
        /// </summary>
        public static readonly PropertyDef<IMovementChecker> MovementChecker = new PropertyDef<IMovementChecker>(nameof(MovementChecker), new MovementChecker());
        }
    }
