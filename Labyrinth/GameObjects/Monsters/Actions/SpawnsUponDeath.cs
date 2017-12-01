namespace Labyrinth.GameObjects.Actions
    {
    class SpawnsUponDeath : BaseBehaviour
        {
        public override void Perform()
            {
            if (this.Monster.IsEgg)
                return;
            var newDiamondDemon = GlobalServices.GameState.AddDiamondDemon(this.Monster.Position);
            GlobalServices.GameState.AddDiamondDemon(this.Monster.Position);
            newDiamondDemon.PlaySound(GameSound.MonsterShattersIntoNewLife);
            }
        }
    }
