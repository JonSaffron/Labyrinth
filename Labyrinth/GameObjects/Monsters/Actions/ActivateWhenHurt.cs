namespace Labyrinth.GameObjects.Actions
    {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Not a behavior of the original game, but seems like a sensible thing to have</remarks>
    class ActivateWhenHurt : BaseBehaviour
        {
        public override void Perform()
            {
            this.Monster.IsActive = true;
            this.Monster.Behaviours.Remove<ActivateWhenHurt>();
            }
        }
    }
