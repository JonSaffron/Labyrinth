﻿using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Monsters.Actions
    {
    public abstract class BaseAction
        {
        protected readonly Player Player = GlobalServices.GameState.Player;
        protected readonly IRandomess Random = GlobalServices.Randomess;
        protected Monster Monster { get; private set; }

        public virtual void Init([NotNull] Monster monster)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public abstract void PerformAction();

        protected void PlaySound(GameSound gameSound)
            {
            this.Monster.PlaySound(gameSound);
            }

        protected bool IsInSameRoom()
            {
            var result = MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster);
            return result;
            }
        }
    }
