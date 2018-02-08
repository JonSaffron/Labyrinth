namespace Labyrinth.GameObjects.Actions
    {
    class StartsShootingWhenHurt : BaseBehaviour
        {
        public override void Perform()
            {
            this.Monster.AddShootsAtPlayerBehaviour();
            this.Monster.Behaviours.Remove<StartsShootingWhenHurt>();
            }
        }
    }
