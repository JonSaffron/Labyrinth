namespace Labyrinth
    {
    class MonsterMovementFactory : IMonsterMovementFactory
        {
        private IMonsterMovement _fullPursuit;
        private IMonsterMovement _cautiousPursuit;
        private IMonsterMovement _semiAggressive;
        private IMonsterMovement _placid;

        public IMonsterMovement StandardPatrolling(Direction initialDirection)
            {
            var result = new StandardPatrolling(initialDirection);
            return result;
            }

        public IMonsterMovement StandardRolling(Direction inititalDirection)
            {
            var result = new StandardRolling(inititalDirection);
            return result;
            }

        public IMonsterMovement FullPursuit()
            {
            var result = this._fullPursuit ?? (this._fullPursuit = new FullPursuit());
            return result;
            }

        public IMonsterMovement Cautious()
            {
            var result = this._cautiousPursuit ?? (this._cautiousPursuit = new CautiousPursuit());
            return result;
            }

        public IMonsterMovement SemiAggressive()
            {
            var result = this._semiAggressive ?? (this._semiAggressive = new SemiAggressive());
            return result;
            }

        public IMonsterMovement Placid()
            {
            var result = this._placid ?? (this._placid = new Placid());
            return result;
            }

        public IMonsterMovement KillerCubeRedMovement()
            {
            var result = new KillerCubeRedMovement();
            return result;
            }

        public IMonsterMovement RotaFloaterCyanMovement()
            {
            var result = new RotaFloatCyanMovement();
            return result;
            }
        }

    class RotaFloatCyanMovement : IMonsterMovement
        {
        public Direction DetermineDirection(GameObjects.Monster monster)
            {
                throw new System.NotImplementedException();
            }
        }
    }
