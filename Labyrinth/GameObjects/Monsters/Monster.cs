using System;
using Labyrinth.Annotations;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Monster : MovingItem
        {
        protected static readonly Random MonsterRandom = new Random();
        
        private MonsterMobility _mobility;
        protected abstract Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility);
        private Func<Monster, World, Direction> _determineDirection;

        public ChangeRooms ChangeRooms { get; set; }
        private MonsterState _monsterState = MonsterState.Normal;
        [CanBeNull] private GameTimer _hatchingTimer;
        protected bool Flitters { private get; set; }
        private bool _flitterFlag;
        public bool LaysEggs { private get; set; }
        public bool LaysMushrooms { private get; set; }
        public bool SplitsOnHit { private get; set; }
        public bool IsActive { get; set; }
        public bool ShotsBounceOff { get; protected set; }
        public MonsterShootBehaviour MonsterShootBehaviour { private get; set;}
        
        [NotNull]
        private IMonsterWeapon _weapon = new StandardMonsterWeapon();

        private Animation _normalAnimation;
        private readonly Animation _eggAnimation;
        private readonly Animation _hatchingAnimation;

        private readonly int _originalEnergy;

        private double _stepTime;

        protected Monster(World world, Vector2 position, int energy) : base(world, position)
            {
            this.Energy = energy;
            this._originalEnergy = energy;
            
            this._eggAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/Egg", 3);
            this._hatchingAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/Egg", 1);
            }
            
        public static Monster Create(string type, World world, Vector2 position, int energy)
            {
            switch (type)
                {
                case "ThresherBrown": return new ThresherBrown(world, position, energy);
                case "RotaFloaterBrown": return new RotaFloaterBrown(world, position, energy);
                case "DeathCube": return new DeathCube(world, position, energy);
                case "RotaFloaterCyan": return new RotaFloaterCyan(world, position, energy);
                case "FlitterbugRed": return new FlitterbugRed(world, position, energy);
                case "KillerCubeGreen": return new KillerCubeGreen(world, position, energy);
                case "ThresherCyan": return new ThresherCyan(world, position, energy);
                case "Butterfly": return new Butterfly(world, position, energy);
                case "KillerCubeRed": return new KillerCubeRed(world, position, energy);
                case "FlitterbugCyan": return new FlitterbugCyan(world, position, energy);
                case "DiamondDemon": return new DiamondDemon(world, position, energy);
                case "FlitterbugBrown": return new FlitterbugBrown(world, position, energy);
                case "CrazyCrawler": return new CrazyCrawler(world, position, energy);
                case "TigerMoth": return new TigerMoth(world, position, energy);
                case "Joker": return new Joker(world, position, energy);
                default: throw new InvalidOperationException("No handler exists for creating monster " + type);
                }
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

        public void SetDelayBeforeHatching(int value)
            {
            this.MonsterState = MonsterState.Egg;
            this._hatchingTimer = new GameTimer(this.World.Game, value*AnimationPlayer.GameClockResolution, EggIsHatching, false);
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Hatching;
            this.World.Game.SoundPlayer.Play(GameSound.EggHatches, EggHasHatched);
            }

        private void EggHasHatched(object sender, SoundEffectFinishedEventArgs args)
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

        protected virtual Monster Clone()
            {
            throw new NotImplementedException();
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
                this.World.Game.SoundPlayer.Play(gs);
                return;
                }

            this.World.Game.SoundPlayer.Play(GameSound.MonsterDies);
            this.World.AddBang(this.Position, BangType.Long);
            if (this.SplitsOnHit)
                {
                for (int i = 1; i <= 2; i++)
                    this.World.AddDiamondDemon(this);
                this.World.Game.SoundPlayer.Play(GameSound.MonsterShattersIntoNewLife);
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

        private bool SetDirectionAndDestination(World w)
            {
            if (this.Mobility == MonsterMobility.Static || this.IsEgg)
                {
                this.StandStill();
                return false;
                }   

            if (this._determineDirection == null)
                throw new InvalidOperationException("Direction function not set.");
            Direction d = this._determineDirection(this, this.World);
            if (d == Direction.None)
                throw new InvalidOperationException("The monster's DetermineDirection routine should not return None.");
            
            d = MonsterMovement.UpdateDirectionWhereMovementBlocked(this, w, d);
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
            this.OriginalPosition = this.Position;

            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this, this.World);
            
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
                if (_stepTime >= AnimationPlayer.GameClockResolution)
                    {
                    _stepTime -= AnimationPlayer.GameClockResolution;
                    DoMonsterAction(inSameRoom);
                    }

                if (!this.IsMoving)
                    {
                    if (!this.SetDirectionAndDestination(this.World))
                        break;

                    if (this.Flitters)
                        {
                        this._flitterFlag = !this._flitterFlag;
                        }
                    }

                this.TryToCompleteMoveToTarget(ref remainingTime);
                result = true;
                }

            while (_stepTime >= AnimationPlayer.GameClockResolution)
                {
                _stepTime -= AnimationPlayer.GameClockResolution;
                DoMonsterAction(inSameRoom);
                }
            return result;
            }

        private void DoMonsterAction(bool inSameRoom)
            {
            if (this.LaysMushrooms && !this.IsEgg && this.World.GameObjects.DoesShotExist() && inSameRoom && (MonsterRandom.Next(256) & 1) == 0 && IsDirectionCompatible(this.World.Player.Direction, this.Direction))
                {
                TilePos tp = this.TilePosition;
                if (!this.World.IsStaticItemOnTile(tp))
                    {
                    this.World.Game.SoundPlayer.Play(GameSound.MonsterLaysMushroom);
                    this.World.AddMushroom(tp);
                    }
                }

            if (this.LaysEggs && inSameRoom && this.World.Player.IsExtant && !this.IsEgg && (MonsterRandom.Next(256) & 0x1f)  == 0)
                {
                TilePos tp = this.TilePosition;
                if (this.World.IsStaticItemOnTile(tp))
                    {
                    this.World.Game.SoundPlayer.Play(GameSound.MonsterLaysEgg);
                    Monster m = this.Clone();
                    m.ResetAnimationPlayerAfterClone();
                    m.Energy = this._originalEnergy;
                    m.SetDelayBeforeHatching((MonsterRandom.Next(256) & 0x1f) + 8);
                    m.LaysEggs = false;
                    this.World.AddMonster(m);
                    }
                }

            if (this.MonsterShootBehaviour == MonsterShootBehaviour.ShootsImmediately && !this.IsEgg && (MonsterRandom.Next(256) & 3) != 0 && this.Energy >= 4 && this.World.Player.IsExtant && MonsterMovement.IsPlayerInSight(this, this.World))
                {
                this._weapon.FireIfYouLike(this, this.World);
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
                Rectangle playerRoom = World.GetContainingRoom(World.Player.Position);
                if (currentRoom == playerRoom)
                    this.World.Game.SoundPlayer.Play(GameSound.MonsterLeavesRoom);
                else if (newRoom == playerRoom)
                    this.World.Game.SoundPlayer.Play(GameSound.MonsterEntersRoom);
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
                var result = AnimationPlayer.BaseSpeed * (this.Flitters && this._flitterFlag ? 2 : 1);
                return result;
                }
            }
        }
    }
