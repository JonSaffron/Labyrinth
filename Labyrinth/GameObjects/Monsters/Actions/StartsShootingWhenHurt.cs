namespace Labyrinth.GameObjects.Actions
    {
    class StartsShootingWhenHurt : BaseBehaviour
        {
        public override void Perform()
            {
            var behaviour = new ShootsAtPlayer(new StandardMonsterWeapon(this.Monster));
            behaviour.Init(this.Monster);
            this.Monster.MovementBehaviours.Add(behaviour);
            this.Monster.InjuryBehaviours.Remove<StartsShootingWhenHurt>();
            }
        }
    }
