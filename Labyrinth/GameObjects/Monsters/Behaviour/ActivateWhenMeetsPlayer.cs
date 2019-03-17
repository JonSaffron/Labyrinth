using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Behaviour
    {
    /// <summary>
    /// Gets the monster moving if it gets hurt
    /// </summary>
    /// <remarks>Not a behaviour of the original game, but seems like a sensible thing to have</remarks>
    class ActivateWhenMeetsPlayer : BaseBehaviour, IMovementBehaviour
        {
        public override void Perform()
            {
            if (this.Monster.IsActive)
                {
                this.RemoveMe();
                return;
                }

            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster);
            if (inSameRoom)
                {
                this.Monster.Activate();
                this.RemoveMe();
                }
            }
        }
    }
