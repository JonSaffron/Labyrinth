﻿namespace Labyrinth.GameObjects.Behaviour
    {
    /// <summary>
    /// Gets the monster moving if it gets hurt
    /// </summary>
    /// <remarks>Not a behaviour of the original game, but seems like a sensible thing to have</remarks>
    class ActivateWhenHurt : BaseBehaviour, IInjuryBehaviour
        {
        public ActivateWhenHurt(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public ActivateWhenHurt()
            {
            // nothing to do
            }

        public override void Perform()
            {
            this.Monster.Activate();
            this.RemoveMe();
            }
        }
    }
