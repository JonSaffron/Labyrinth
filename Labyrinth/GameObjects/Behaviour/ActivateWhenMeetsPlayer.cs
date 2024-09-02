using Labyrinth.GameObjects.Motility;

namespace Labyrinth.GameObjects.Behaviour
    {
    /// <summary>
    /// Gets the monster to start moving when it encounters the player
    /// </summary>
    internal class ActivateWhenMeetsPlayer : BaseBehaviour, IMovementBehaviour
        {
        public override void Perform()
            {
            if (this.Monster.IsActive)
                {
                this.RemoveMe();
                return;
                }

            bool inSameRoom = this.Monster.IsPlayerInSameRoom();
            if (inSameRoom)
                {
                this.Monster.Activate();
                this.RemoveMe();
                }
            }
        }
    }
