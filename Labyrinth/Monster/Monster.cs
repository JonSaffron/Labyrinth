using System;
using Labyrinth.Annotations;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    abstract class Monster : MovingItem
        {
        protected static readonly Random MonsterRandom = new Random();
        
        private MonsterMobility _mobility;
        protected abstract Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility);
        private Func<Monster, World, Direction> _determineDirection;

        public ChangeRooms ChangeRooms { get; set; }
        private MonsterState _monsterState;
        private int _delayBeforeHatching;
        [CanBeNull] private GameTimer _hatchingTimer;
        protected bool Flitters { private get; set; }
        private bool _flitterFlag;
        public bool LaysEggs { private get; set; }
        public bool LaysMushrooms { private get; set; }
        public bool SplitsOnHit { private get; set; }
        public bool IsActive { get; set; }
        public bool ShotsBounceOff { get; protected set; }
        public MonsterShootBehaviour MonsterShootBehaviour { private get; set;}
        
        private Animation _normalAnimation;
        private readonly Animation _eggAnimation;
        private readonly Animation _hatchingAnimation;

        private readonly int _originalEnergy;

        private float _stepTime;

        protected Monster(World world, Vector2 position, int energy) : base(world, position)
            {
            this.Energy = energy;
            this._originalEnergy = energy;
            
            this._eggAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/Egg", 3);
            this._hatchingAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/Egg", 1);
            this.MonsterState = MonsterState.Normal;
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

        public int DelayBeforeHatching
            {
            get
                {
                var result = this._delayBeforeHatching;
                return result;
                }
            set
                {
                this._delayBeforeHatching = value;
                this.MonsterState = MonsterState.Egg;
                this._hatchingTimer = new GameTimer(this.World.Game, value * AnimationPlayer.GameClockResolution, EggIsHatching, false);
                }
            }

        private void EggIsHatching(object sender, EventArgs args)
            {
            this.MonsterState = MonsterState.Hatching;
            this.World.Game.SoundLibrary.Play(GameSound.EggHatches, EggHasHatched);
            }

        private void EggHasHatched(object sender, SoundEffectFinishedEventArgs args)
            {
            this.MonsterState = MonsterState.Normal;
            }

        public Animation NormalAnimation
            {
            get
                {
                var result = this._normalAnimation;
                return result;
                }
            set
                {
                this._normalAnimation = value;
                if (this.MonsterState == MonsterState.Normal)
                    this.Ap.PlayAnimation(value);
                }
            }

        public MonsterState MonsterState
            {
            get
                {
                var result = this._monsterState;
                return result;
                }
            private set
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
                        animation = this.NormalAnimation;
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
                this.World.Game.SoundLibrary.Play(gs);
                return;
                }

            this.World.Game.SoundLibrary.Play(GameSound.MonsterDies);
            this.World.AddLongBang(this.Position);
            if (this.SplitsOnHit)
                {
                for (int i = 1; i <= 2; i++)
                    this.World.AddDiamondDemon(this);
                this.World.Game.SoundLibrary.Play(GameSound.MonsterShattersIntoNewLife);
                }
            }
        
        private void SetDirectionAndDestination(World w)
            {
            if (this.Mobility == MonsterMobility.Static || this.IsEgg)
                {
                this.Direction = Direction.None;
                return;
                }   

            if (this._determineDirection == null)
                throw new InvalidOperationException("Direction function not set.");
            Direction d = this._determineDirection(this, this.World);
            
            if (d == Direction.None)
                throw new InvalidOperationException();
            
            this.Direction = MonsterMovement.UpdateDirectionWhereMovementBlocked(this, w, d);
            if (this.Direction == Direction.None)
                {
                this.MovingTowards = Vector2.Zero;
                }
            else
                {
                TilePos tp = this.TilePosition;
                TilePos potentiallyMovingTowardsTile = TilePos.GetPositionAfterOneMove(tp, this.Direction);
                Vector2 potentiallyMovingTowards = potentiallyMovingTowardsTile.ToPosition();
                this.MovingTowards = potentiallyMovingTowards;
                }
            }
        
        /// <summary>
        /// Handles input, performs physics, and animates the sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this, this.World);
            
            if (this.IsEgg && this._hatchingTimer != null)
                this._hatchingTimer.Enabled = inSameRoom;

            // if inactive and in same room then activate
            if (!this.IsActive && inSameRoom)
                this.IsActive = true;

            if (!this.IsActive)
                return false;

            bool result = false;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _stepTime += elapsed;
            float remainingMovement = (CurrentVelocity * (_flitterFlag ? 2 : 1)) * elapsed;
            bool needToChooseDirection = (this.Direction == Direction.None);
            while (remainingMovement > 0)
                {
                if (needToChooseDirection)
                    {
                    this.SetDirectionAndDestination(this.World);
                    if (this.Flitters)
                        this._flitterFlag = !this._flitterFlag;
                    remainingMovement *= this._flitterFlag ? 2.0f : 0.5f;

                    if (_stepTime >= AnimationPlayer.GameClockResolution)
                        {
                        _stepTime -= AnimationPlayer.GameClockResolution;
                        DoMonsterAction(inSameRoom);
                        }

                    }

                if (this.Direction == Direction.None)
                    {
                    remainingMovement = 0;
                    }
                else
                    {
                    this.ContinueMove(ref remainingMovement, out needToChooseDirection);
                    result = true;
                    }                
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
            if (this.LaysMushrooms && !this.IsEgg && this.World.ShotExists() && inSameRoom && (MonsterRandom.Next(256) & 1) == 0 && IsDirectionCompatible(this.World.Player.Direction, this.Direction))
                {
                TilePos tp = this.TilePosition;
                if (!this.World.IsStaticItemOnTile(tp))
                    {
                    this.World.Game.SoundLibrary.Play(GameSound.MonsterLaysMushroom);
                    this.World.AddMushroom(tp);
                    }
                }

            if (this.LaysEggs && inSameRoom && this.World.Player.IsExtant && !this.IsEgg && (MonsterRandom.Next(256) & 0x1f)  == 0)
                {
                TilePos tp = this.TilePosition;
                if (this.World.IsStaticItemOnTile(tp))
                    {
                    this.World.Game.SoundLibrary.Play(GameSound.MonsterLaysEgg);
                    Monster m = this.Clone();
                    m.ResetAnimationPlayerAfterClone();
                    m.Energy = this._originalEnergy;
                    m.DelayBeforeHatching = (MonsterRandom.Next(256) & 0x1f) + 8;
                    m.LaysEggs = false;
                    this.World.AddMonster(m);
                    }
                }

            if (this.MonsterShootBehaviour == MonsterShootBehaviour.ShootsImmediately && !this.IsEgg && (MonsterRandom.Next(256) & 3) != 0 && this.Energy >= 4 && this.World.Player.IsExtant && MonsterMovement.IsPlayerInSight(this, this.World))
                {
                CheckToFire();
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

        protected override void ContinueMove(ref float maxMovementRemaining, out bool hasArrivedAtDestination)
            {
            Rectangle currentRoom = World.GetContainingRoom(this.TilePosition);
            
            base.ContinueMove(ref maxMovementRemaining, out hasArrivedAtDestination);

            Rectangle newRoom = World.GetContainingRoom(this.TilePosition);
            if (currentRoom != newRoom)
                {
                Rectangle playerRoom = World.GetContainingRoom(World.Player.TilePosition);
                if (currentRoom == playerRoom)
                    this.World.Game.SoundLibrary.Play(GameSound.MonsterLeavesRoom);
                else if (newRoom == playerRoom)
                    this.World.Game.SoundLibrary.Play(GameSound.MonsterEntersRoom);
                }
            }

        private void CheckToFire()
            {
            TilePos tp = this.TilePosition;
            TilePos playerPosition = this.World.Player.TilePosition;
            
            Direction firingDirection = Direction.None;
            int xDiff = Math.Sign(tp.X - playerPosition.X);
            int yDiff = Math.Sign(tp.Y - playerPosition.Y);
            if (xDiff == 0)
                {
                switch (yDiff)
                    {
                    case 1: firingDirection = Direction.Up; break;
                    case -1: firingDirection = Direction.Down; break;
                    }
                }
            else if (yDiff == 0)
                {
                switch (xDiff)
                    {
                    case 1: firingDirection = Direction.Left; break;
                    case -1: firingDirection = Direction.Right; break;
                    }
                }
                
            if (firingDirection == Direction.None)
                return;

            // Do any walls intervene?
            TilePos startPos = TilePos.GetPositionAfterOneMove(this.TilePosition, firingDirection);
            TilePos testPos = startPos;
            while (true)
                {
                bool isClear = this.World.CanTileBeOccupied(testPos, false);
                if (!isClear)
                    return;
                testPos = TilePos.GetPositionAfterOneMove(testPos, firingDirection);
                if (testPos == playerPosition)
                    break;
                }
                
            this.World.AddShot(ShotType.Monster, startPos.ToPosition(), this.Energy >> 2, firingDirection);
            this.World.Game.SoundLibrary.Play(GameSound.MonsterShoots);
            }
        }
    }
