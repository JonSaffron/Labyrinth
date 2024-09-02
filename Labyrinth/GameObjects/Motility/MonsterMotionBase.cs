using System;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    internal abstract class MonsterMotionBase : IMonsterMotion
        {
        protected readonly Monster Monster;

        protected MonsterMotionBase(Monster monster)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public abstract ConfirmedDirection GetDirection();

        protected ConfirmedDirection GetConfirmedDirection(IDirectionChosen selectedDirection)
            {
            if (selectedDirection is ConfirmedDirection || selectedDirection.Direction == Direction.None)
                return selectedDirection.Confirm();
            
            this.Monster.ConfirmDirectionToMoveIn(selectedDirection, out ConfirmedDirection feasibleDirection);
            return feasibleDirection;
            }
        }
    }
