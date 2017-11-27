﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.GameObjects.Monsters.Actions;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

// todo should monsters be a combination of the generic monster class and a breed class?

namespace Labyrinth.GameObjects
    {
    public abstract class Monster : MovingItem, IMonster
        {
        private MonsterMobility _mobility;
        protected abstract IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility);
        [CanBeNull] private IMonsterMotion _determineDirection;
        public Direction InitialDirection = Direction.None;

        public ChangeRooms ChangeRooms { get; set; }
        private MonsterState _monsterState = MonsterState.Normal;
        [CanBeNull] private GameTimer _hatchingTimer;

        public bool SplitsOnHit { get; set; }
        public bool IsActive { get; set; }
        public bool ShotsBounceOff { get; set; }
        public bool ShootsOnceProvoked { get; set; }

        protected List<BaseAction> _actions;

        private IMonsterWeapon _weapon;

        private Animation _normalAnimation;
        private static readonly Animation EggAnimation;
        private static readonly Animation HatchingAnimation;

        public readonly int OriginalEnergy;

        private IEnumerator<bool> _movementIterator;
        private double _remainingTime;

        static Monster()
            {
            EggAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 3);
            HatchingAnimation = Animation.LoopingAnimation("Sprites/Monsters/Egg", 1);
            }

        protected Monster(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position)
            {
            this.Energy = energy;
            this.OriginalEnergy = energy;
            this.CurrentSpeed = Constants.BaseSpeed;
            }
            
        public MonsterMobility Mobility
            {
            get
                {
                return this._mobility;
                }
            set
                {
                this._mobility = value;
                SetMonsterMotion();
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
            if (this.IsExtant)
                PlaySoundWithCallback(GameSound.EggHatches, EggHatches);
            }

        public void EggHatches(object sender, EventArgs args)
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
                        animation = EggAnimation;
                        break;
                    case MonsterState.Hatching:
                        animation = HatchingAnimation;
                        break;
                    case MonsterState.Normal:
                        animation = this._normalAnimation;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                    }
                this.Ap.PlayAnimation(animation);
                this._monsterState = value;
                SetMonsterMotion();
                }
            }

        private void SetMonsterMotion()
            {
            this._determineDirection = this.IsEgg || this.Mobility == MonsterMobility.Stationary ? new Stationary(this) : GetMethodForDeterminingDirection(this.Mobility);
            }

        public bool IsStatic => this.Mobility == MonsterMobility.Stationary || this.IsEgg;

        public override void ReduceEnergy(int energyToRemove)
            {
            base.ReduceEnergy(energyToRemove);
            if (this.IsAlive())
                {
                if (!this.IsActive)
                    this.IsActive = true;

                if (this.ShootsOnceProvoked)
                    this.ShootsAtPlayer = true;

                if (this.Mobility == MonsterMobility.Patrolling)
                    this.Mobility = MonsterMobility.Placid;
                return;
                }

            var bang = GlobalServices.GameState.AddBang(this.Position, BangType.Long);
            bang.PlaySound(GameSound.MonsterDies);
            
            if (this.SplitsOnHit && !this.IsEgg)
                {
                for (int i = 1; i <= 2; i++)
                    GlobalServices.GameState.AddDiamondDemon(this.Position);
                bang.PlaySound(GameSound.MonsterShattersIntoNewLife);
                }
            }

        public override int DrawOrder
            {
            get
                {
                var result = this.IsStatic ? SpriteDrawOrder.StaticMonster : SpriteDrawOrder.MovingMonster;
                return (int) result;
                }
            }

        private bool SetDirectionAndDestination()
            {
            if (this._determineDirection == null)
                throw new InvalidOperationException("Determine Direction object reference not set.");

            var result = this._determineDirection.SetDirectionAndDestination();
            return result;
            }

        /// <summary>
        /// Update the monster's position, and run its behaviours
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            bool inSameRoom = MonsterMovement.IsPlayerInSameRoomAsMonster(this);

            if (this.IsEgg && this._hatchingTimer != null)
                this._hatchingTimer.Enabled = inSameRoom;

            if (inSameRoom)
                this.IsActive = true;

            if (!this.IsActive)
                return false;

            // move the monster
            this._remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
            if (this._movementIterator == null)
                this._movementIterator = this._movementIterator = Move().GetEnumerator();
            this._movementIterator.MoveNext();
            var result = this._movementIterator.Current;
            return result;
            }

        public override void ResetPosition(Vector2 position)
            {
            base.ResetPosition(position);
            // it's essential to reset the iterator 
            this._movementIterator = null;
            }

        private IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            double timeStationary = 0;
            while (true)
                {
                if (this.SetDirectionAndDestination())
                    {
                    hasMovedSinceLastCall = true;
                    timeStationary = 0;
                    while (true)
                        {
                        if (this.TryToCompleteMoveToTarget(ref this._remainingTime))
                            break;
                        
                        yield return true;  // we have moved
                        }

                    DoMonsterActions();
                    }
                else
                    {
                    timeStationary += this._remainingTime;
                    DoActionsWhilstStationary(ref timeStationary);
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        private void DoActionsWhilstStationary(ref double timeStationary)
            {
            while (true)
                {
                var timeItWouldTakeToMakeAMove = Constants.TileLength / (double) this.StandardSpeed;
                if (timeStationary < timeItWouldTakeToMakeAMove)
                    break;

                timeStationary -= timeItWouldTakeToMakeAMove;
                DoMonsterActions();
                }
            }

        private void DoMonsterActions()
            {
            if (this._actions == null)
                return;
            foreach (var action in this._actions)
                action.PerformAction();
            }

        public void FireWeapon()
            {
            _weapon?.FireIfYouLike();
            }

        /// <inheritdoc cref="MovingItem.TryToCompleteMoveToTarget" />
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
        public override ObjectSolidity Solidity => this.IsStatic ? ObjectSolidity.Stationary : ObjectSolidity.Insubstantial;

        public int CurrentSpeed { get; set; }
        public override decimal StandardSpeed => this.CurrentSpeed;

        public bool Flitters
            {
            get => this._actions != null && this._actions.OfType<Flitter>().Any();
            set => SetAction<Flitter>(ref this._actions, value);
            }
        
        public bool LaysMushrooms
            {
            get => this._actions != null && this._actions.OfType<LaysMushroom>().Any();
            set => SetAction<LaysMushroom>(ref this._actions, value);
            }

        public bool LaysEggs
            {
            get => this._actions != null && this._actions.OfType<LaysEgg>().Any();
            set => SetAction<LaysEgg>(ref this._actions, value);
            }

        public bool ShootsAtPlayer
            {
            get => this._actions != null && this._actions.OfType<ShootsAtPlayer>().Any();

            set
                {
                SetAction<ShootsAtPlayer>(ref this._actions, value);
                this._weapon = value ? new StandardMonsterWeapon(this) : null;
                }
            }

        private void SetAction<T>(ref List<BaseAction> actionList, bool enable) where T : BaseAction, new()
            {
            if (enable)
                AddAction<T>(ref actionList);
            else
                actionList?.RemoveAll(item => item is T);
            }

        private void AddAction<T>(ref List<BaseAction> actionList) where T: BaseAction, new() 
            {
            if (actionList == null)
                actionList = new List<BaseAction>();
            else
                {
                if (actionList.OfType<T>().Any())
                    return;
                }
            var action = new T();
            action.Init(this);
            actionList.Add(action);
            }

        protected override bool CanChangeRooms => this.ChangeRooms == ChangeRooms.FollowsPlayer || this.ChangeRooms == ChangeRooms.MovesRoom;
        }
    }
