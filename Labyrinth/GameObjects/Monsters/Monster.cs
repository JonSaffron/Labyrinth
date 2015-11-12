using System;
using Labyrinth.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public abstract class Monster : MovingItem, IMonster
        {
        protected static readonly Random MonsterRandom = new Random();
        
        private MonsterMobility _mobility;
        protected abstract IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility);
        private IMonsterMovement _determineDirection;
        public Direction InitialDirection = Direction.None;

        public ChangeRooms ChangeRooms { get; set; }
        private MonsterState _monsterState = MonsterState.Normal;
        [CanBeNull] private GameTimer _hatchingTimer;
        protected bool Flitters { private get; set; }
        private bool _flitterFlag;

        public bool LaysMushrooms { private get; set; }
        public bool SplitsOnHit { private get; set; }
        public bool IsActive { get; set; }
        public bool ShotsBounceOff { get; protected set; }
        public MonsterShootBehaviour MonsterShootBehaviour { get; set; }
        
        [NotNull]
        private IMonsterWeapon _weapon = new StandardMonsterWeapon();

        private Animation _normalAnimation;
        private readonly Animation _eggAnimation;
        private readonly Animation _hatchingAnimation;

        protected readonly int OriginalEnergy;

        private double _stepTime;
        private bool _laysEggs;

        protected Monster(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position)
            {
            this.Energy = energy;
            this.OriginalEnergy = energy;
            
            this._eggAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 3);
            this._hatchingAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 1);
            }
            
        public MonsterMobility Mobility
            {
            get
                {
                var result = this._mobility;
                return result;
                }
            set
                {
                this._determineDirection = GetMethodForDeterminingDirection(value);
                this._mobility = value;
                }
            }

        public bool IsEgg
            {
            get
                {
                var monsterState = this.MonsterState;
                switch (monsterState)
                    {
                    case MonsterState.Egg:
                    case MonsterState.Hatching:
                        return true;
                    }
                return false;
                }
            }

        public void SetDelayBeforeHatching(int gameTicks)
            {
            this.MonsterState = MonsterState.Egg;
            var timeSpan = TimeSpan.FromSeconds(gameTicks * Constants.GameClockResolution);
            this._hatchingTimer = GameTimer.AddGameTimer(timeSpan, EggIsHatching, false);
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Hatching;
            PlaySoundWithCallback(GameSound.EggHatches, EggHasHatched);
            }

        private void EggHasHatched(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Normal;
            }

        protected void SetNormalAnimation(Animation value)
            {
            this._normalAnimation = value;
            if (this.MonsterState == MonsterState.Normal)
                this.Ap.PlayAnimation(value);
            }

        private MonsterState MonsterState
            {
            get
                {
                var result = this._monsterState;
                return result;
                }
            set
                {
                Animation animation;
                switch (value)
                    {
                    case MonsterState.Egg:
                        animation = this._eggAnimation;
                        break;
                    case MonsterState.Hatching:
                        animation = this._hatchingAnimation;
                        break;
                    case MonsterState.Normal:
                        animation = this._normalAnimation;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                    }
                this.Ap.PlayAnimation(animation);
                this._monsterState = value;
                }
            }
        
        public bool IsStill
            {
            get
                {
                return this.Mobility == MonsterMobility.Static || this.IsEgg;
                }
            }
        
        public override void ReduceEnergy(int energyToRemove)
            {
            base.ReduceEnergy(energyToRemove);
            if (this.IsAlive())
                {
                var gs = this.IsEgg ? GameSound.PlayerShootsAndInjuresEgg : GameSound.PlayerShootsAndInjuresMonster;
                this.PlaySound(gs);

                if (!this.IsActive)
                    this.IsActive = true;

                if (this.Mobility == MonsterMobility.Patrolling)
                    this.Mobility = MonsterMobility.Placid;
                return;
                }

            this.PlaySound(GameSound.MonsterDies);
            GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            
            if (this.SplitsOnHit)
                {
                for (int i = 1; i <= 2; i++)
                    GlobalServices.GameState.AddDiamondDemon(this.Position);
                this.PlaySound(GameSound.MonsterShattersIntoNewLife);
                }
            }

        public override int DrawOrder
            {
            get
                {
                var result = this.IsStill ? SpriteDrawOrder.StaticMonster : SpriteDrawOrder.MovingMonster;
                return (int) result;
                }
            }

        private bool SetDirectionAndDestination()
            {
            if (this.Mobility == MonsterMobility.Static || this.IsEgg)
                {
                this.StandStill();
                return false;
                }   

            if (this._determineDirection == null)
                throw new InvalidOperationException("Determine Direction object reference not set.");
            Direction d = this._determineDirection.DetermineDirection(this);

            if (d == Direction.None)
                {
                this.StandStill();
                return false;
                }

            this.Move(d, this.StandardSpeed);
            return true;
            }
        
        /// <summary>
        /// Handles input, performs physics, and animates the sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this);
            
            if (this.IsEgg && this._hatchingTimer != null)
                this._hatchingTimer.Enabled = inSameRoom;

            // if inactive and in same room then activate
            if (!this.IsActive && inSameRoom)
                this.IsActive = true;

            if (!this.IsActive)
                return false;

            bool result = false;
            var remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
            this._stepTime += remainingTime;
            while (remainingTime > 0)
                {
                if (_stepTime >= Constants.GameClockResolution)
                    {
                    _stepTime -= Constants.GameClockResolution;
                    DoMonsterAction(inSameRoom);
                    }

                if (!this.IsMoving)
                    {
                    if (!this.SetDirectionAndDestination())
                        break;

                    if (this.Flitters)
                        {
                        this._flitterFlag = !this._flitterFlag;
                        }
                    }

                this.TryToCompleteMoveToTarget(ref remainingTime);
                result = true;
                }

            while (_stepTime >= Constants.GameClockResolution)
                {
                _stepTime -= Constants.GameClockResolution;
                DoMonsterAction(inSameRoom);
                }
            return result;
            }

        private void DoMonsterAction(bool inSameRoom)
            {
            var player = GlobalServices.GameState.Player;

            if (this.LaysMushrooms && !this.IsEgg && GlobalServices.GameState.DoesShotExist() && inSameRoom && (MonsterRandom.Next(256) & 1) == 0 && IsDirectionCompatible(player.Direction, this.Direction))
                {
                TilePos tp = this.TilePosition;
                if (!GlobalServices.GameState.IsStaticItemOnTile(tp))
                    {
                    this.PlaySound(GameSound.MonsterLaysMushroom);
                    GlobalServices.GameState.AddMushroom(tp);
                    }
                }

            if (this.LaysEggs && inSameRoom && GlobalServices.GameState.Player.IsExtant && !this.IsEgg && (MonsterRandom.Next(256) & 0x1f)  == 0)
                {
                TilePos tp = this.TilePosition;
                if (!GlobalServices.GameState.IsStaticItemOnTile(tp))
                    {
                    this.PlaySound(GameSound.MonsterLaysEgg);
                    Monster m = ((ILayEggs) this).LayAnEgg();
                    m.Mobility = this.Mobility;
                    m.ChangeRooms = this.ChangeRooms;
                    m.LaysMushrooms = this.LaysMushrooms;
                    m.SplitsOnHit = this.SplitsOnHit;
                    m.IsActive = true;
                    m.ShotsBounceOff = this.ShotsBounceOff;
                    m.MonsterShootBehaviour = this.MonsterShootBehaviour;
                    m.SetDelayBeforeHatching((MonsterRandom.Next(256) & 0x1f) + 8);
                    m.LaysEggs = false;
                    GlobalServices.GameState.AddMonster(m);
                    }
                }

            if (this.MonsterShootBehaviour == MonsterShootBehaviour.ShootsImmediately && !this.IsEgg && (MonsterRandom.Next(256) & 3) != 0 && this.Energy >= 4 && player.IsExtant && MonsterMovement.IsPlayerInSight(this))
                {
                this._weapon.FireIfYouLike(this);
                }
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

        protected override bool TryToCompleteMoveToTarget(ref double timeRemaining)
            {
            Rectangle currentRoom = World.GetContainingRoom(this.Position);
            
            var result = base.TryToCompleteMoveToTarget(ref timeRemaining);

            Rectangle newRoom = World.GetContainingRoom(this.Position);
            if (currentRoom != newRoom)
                {
                Rectangle playerRoom = World.GetContainingRoom(GlobalServices.GameState.Player.Position);
                if (currentRoom == playerRoom)
                    this.PlaySound(GameSound.MonsterLeavesRoom);
                else if (newRoom == playerRoom)
                    this.PlaySound(GameSound.MonsterEntersRoom);
                }
            return result;
            }

        /// <summary>
        /// Gets an indication of how solid the object is
        /// </summary>
        public override ObjectSolidity Solidity
            {
            get
                {
                var result = this.IsStill ? ObjectSolidity.Stationary : ObjectSolidity.Insubstantial;
                return result;
                }
            }

        protected override decimal StandardSpeed
            {
            get
                {
                var result = Constants.BaseSpeed * (this.Flitters && this._flitterFlag ? 2 : 1);
                return result;
                }
            }

        public bool LaysEggs
            {
            private get
                {
                return _laysEggs;
                }
            set
                {
                if (value && !(this is ILayEggs))
                    throw new InvalidOperationException(string.Format("Cannot set a {0} as laying eggs because the ILayEggs interface is not implemented.", this.GetType().Name));
                this._laysEggs = value;
                }
            }
        }
    }
