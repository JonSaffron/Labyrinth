using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Behaviour
    {
    public abstract class BaseBehaviour : IBehaviour
        {
        protected readonly Player Player = GlobalServices.GameState.Player;
        protected readonly IRandomness Random = GlobalServices.Randomness;

        protected Monster Monster { get; private set; }

        protected BaseBehaviour()
            {
            // nothing to do
            }

        // ReSharper disable once UnusedMember.Global - used by reflection
        protected BaseBehaviour([NotNull] Monster monster)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public virtual void Init([NotNull] Monster monster)
            {
            // ReSharper disable once JoinNullCheckWithUsage
            if (monster == null)
                {
                throw new ArgumentNullException(nameof(monster));
                }
            if (this.Monster != null)
                {
                throw new InvalidOperationException("Init already called.");
                }
            this.Monster = monster;
            }

        public abstract void Perform();

        protected void PlaySound(GameSound gameSound)
            {
            this.Monster.PlaySound(gameSound);
            }

        protected bool IsInSameRoom()
            {
            var result = MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster);
            return result;
            }

        protected void RemoveMe()
            {
            this.Monster.Behaviours.Remove(this);
            }
        }
    }
