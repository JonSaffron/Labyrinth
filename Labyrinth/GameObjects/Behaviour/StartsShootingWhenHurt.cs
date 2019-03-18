namespace Labyrinth.GameObjects.Behaviour
    {
    class StartsShootingWhenHurt : BaseBehaviour, IInjuryBehaviour
        {
        public StartsShootingWhenHurt(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public StartsShootingWhenHurt()
            {
            // nothing to do
            }

        public override void Perform()
            {
            this.Monster.Behaviours.Add<ShootsAtPlayer>();
            this.RemoveMe();
            }
        }
    }
