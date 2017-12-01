namespace Labyrinth.GameObjects.Actions
    {
    class ActivateWhenHurt : BaseBehaviour
        {
        public override void Perform()
            {
            this.Monster.IsActive = true;
            this.Monster.InjuryBehaviours.Remove<ActivateWhenHurt>();
            }
        }
    }
