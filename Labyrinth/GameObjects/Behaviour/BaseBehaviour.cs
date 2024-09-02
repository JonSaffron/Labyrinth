using System;
using Labyrinth.GameObjects.Motility;

namespace Labyrinth.GameObjects.Behaviour
    {
    public abstract class BaseBehaviour : IBehaviour
        {
        protected Player Player => GlobalServices.GameState.Player;
        protected readonly IRandomness Random = GlobalServices.Randomness;

        private Monster? _monster;
        protected Monster Monster 
            {
            get
                {
                if (this._monster == null)
                    throw new InvalidOperationException("Monster property has not been set");
                return this._monster;
                }
            }

        protected BaseBehaviour()
            {
            // nothing to do
            }

        // ReSharper disable once UnusedMember.Global - used by reflection
        protected BaseBehaviour(Monster monster)
            {
            this._monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public void Init(Monster monster)
            {
            // ReSharper disable once JoinNullCheckWithUsage
            if (monster == null)
                {
                throw new ArgumentNullException(nameof(monster));
                }
            if (this._monster != null)
                {
                throw new InvalidOperationException("Init already called.");
                }
            this._monster = monster;
            OnInit();
            }

        protected virtual void OnInit()
            {
            // override if necessary
            }

        public abstract void Perform();

        protected void PlaySound(GameSound gameSound)
            {
            this.Monster.PlaySound(gameSound);
            }

        protected bool IsInSameRoom()
            {
            var result = this.Monster.IsPlayerInSameRoom();
            return result;
            }

        protected void RemoveMe()
            {
            this.Monster.Behaviours.Remove(this);
            }
        }
    }
