using Labyrinth.GameObjects.Movement;

namespace Labyrinth.GameObjects.Behaviour
    {
    /// <summary>
    /// Gets the monster moving if it gets hurt
    /// </summary>
    /// <remarks>Not a behaviour of the original game, but seems like a sensible thing to have</remarks>
    class ActivateWhenMeetsPlayer : BaseBehaviour, IInjuryBehaviour
        {
        public ActivateWhenMeetsPlayer(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public override void Perform()
            {
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this.Monster);
            if (inSameRoom)
                {
                this.Monster.IsActive = true;
                this.RemoveMe();
                }
            }
        }
    }
