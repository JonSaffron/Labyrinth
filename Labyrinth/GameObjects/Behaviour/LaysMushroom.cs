using System;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Behaviour
    {
    class LaysMushroom : BaseBehaviour, IMovementBehaviour
        {
        public LaysMushroom(Monster monster) : base(monster)
            {
            // nothing to do
            }

        public LaysMushroom()
            {
            // nothing to do
            }

        public override void Perform()
            {
            if (!ShouldAttemptToLayMushroom())
                return;

            TilePos tp = this.Monster.TilePosition;
            if (!GlobalServices.GameState.IsStaticItemOnTile(tp))
                {
                this.PlaySound(GameSound.MonsterLaysMushroom);
                GlobalServices.GameState.AddMushroom(tp);
                }
            }

        private bool ShouldAttemptToLayMushroom()
            {
            var result =
                GlobalServices.GameState.DoesShotExist()
                && this.IsInSameRoom()
                && this.Random.Test(3)
                && IsDirectionCompatible(this.Player.CurrentMovement.Direction, this.Monster.CurrentMovement.Direction);
            return result;
            }

        private static int GetDirectionNumber(Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return 0;
                case Direction.Right:
                    return 1;
                case Direction.Up:
                    return 2;
                case Direction.Down:
                    return 3;
                case Direction.None:
                    return 4;
                default:
                    throw new InvalidOperationException();
                }
            }

        private static bool IsDirectionCompatible(Direction playerDirection, Direction monsterDirection)
            {
            int p = GetDirectionNumber(playerDirection);
            int m = GetDirectionNumber(monsterDirection);
            int r = p ^ m;              // EOR 0c04
            r &= 2;                     // AND #2
            bool result = (r == 0);     // not compatible if not zero
            return result;
            }
        }
    }
