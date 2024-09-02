using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Labyrinth.DataStructures;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.Input
    {
    internal class PlayerController : GameComponent, IPlayerInput
        {
        private readonly Queue<TimedInstruction> _instructions = new Queue<TimedInstruction>(); 
        private Instruction? _currentInstruction;

        private TimeSpan _totalGameTime;

        public PlayerController(Game game) : base(game)
            {
            }

        public void Enqueue(IEnumerable<TimedInstruction> instructions)
            {
            if (instructions == null) throw new ArgumentNullException(nameof(instructions));
            foreach (var item in instructions)
                {
                if (item.TakesEffectFrom < this._totalGameTime)
                    throw new ArgumentOutOfRangeException(nameof(instructions), "Cannot queue an instruction that occurs before the current time.");
                this._instructions.Enqueue(item);
                }
            }

        public override void Update(GameTime gameTime)
            {
            this._totalGameTime = gameTime.TotalGameTime;
            if (this._instructions.Count == 0)
                return;

            var nextInstruction = this._instructions.Peek();
            if (this._totalGameTime >= nextInstruction.TakesEffectFrom)
                {
                if (this._currentInstruction != null && this._currentInstruction.Direction != Direction.None)
                    throw new InvalidOperationException("Still processing previous instruction.");
                this._currentInstruction = this._instructions.Dequeue().Instruction;
                System.Diagnostics.Trace.WriteLine("ProcessInput changing to: " + this._currentInstruction);
                }
            }

        public PlayerControl Update()
            {
            var ci = this._currentInstruction;
            if (ci == null)
                {
                return PlayerControl.NoAction;
                }

            var direction = Direction.None;
            var p = GlobalServices.GameState.Player;
            if (ci.Direction != Direction.None)
                {
                if (ci.MoveType == MoveType.TurnToFace)
                    {
                    if (p.CurrentDirectionFaced != ci.Direction)
                        {   // turn to face the required direction
                        direction = ci.Direction;
                        }
                    ci.Direction = Direction.None;
                    }
                else if (ci.MoveType == MoveType.TurnAndMove)
                    {
                    if (p.CurrentDirectionFaced != ci.Direction)
                        {   // turn to face the required direction
                        direction = ci.Direction;
                        ci.MoveType = MoveType.WaitingToMove;
                        }
                    else 
                        {
                        direction = ci.Direction;
                        ci.Direction = Direction.None;
                        }
                    }
                else if (ci.MoveType == MoveType.WaitingToMove)
                    {
                    direction = ci.Direction;
                    if (p.WhenCanMoveInDirectionFaced <= this._totalGameTime)
                        {
                        ci.Direction = Direction.None;
                        }
                    }
                }
                
            var fireStatus1 = ci.FireStatus1;
            if (fireStatus1 == FiringState.Pulse)
                ci.FireStatus1 = FiringState.None;

            var fireStatus2 = ci.FireStatus2;
            if (fireStatus2 == FiringState.Pulse)
                ci.FireStatus2 = FiringState.None;

            return new PlayerControl(direction, fireStatus1, fireStatus2);
            }

        public bool HasFinishedQueue
            {
            get
                {
                var result = this._instructions.Count == 0 && (this._currentInstruction == null || this._currentInstruction.Direction == Direction.None);
                return result;
                }
            }

        public class TimedInstruction
            {
            public readonly TimeSpan TakesEffectFrom;
            public readonly Instruction Instruction;

            public TimedInstruction(TimeSpan takesEffectFrom, Instruction instruction)
                {
                this.TakesEffectFrom = takesEffectFrom;
                this.Instruction = instruction;
                }

            [PublicAPI]
            public static TimedInstruction InitialState()
                {
                var result = new TimedInstruction(TimeSpan.Zero, Instruction.DoNothingInstruction());
                return result;
                }

            public override string ToString()
                {
                var result = $"At {this.TakesEffectFrom}, {this.Instruction}";
                return result;
                }
            }

        public class Instruction
            {
            public Direction Direction;
            public MoveType MoveType;
            public FiringState FireStatus1;
            public FiringState FireStatus2;

            public static Instruction Move(Direction direction)
                {
                return new Instruction(direction, MoveType.TurnAndMove, FiringState.None, FiringState.None);
                }

            public static Instruction Fire(Direction direction)
                {
                return new Instruction(direction, MoveType.TurnToFace, FiringState.Pulse, FiringState.None);
                }

            private Instruction(Direction direction, MoveType moveType, FiringState fireStatus1, FiringState fireStatus2)
                {
                this.Direction = direction;
                this.MoveType = moveType;
                this.FireStatus1 = fireStatus1;
                this.FireStatus2 = fireStatus2;
                }

            public static Instruction DoNothingInstruction()
                {
                var result = new Instruction(Direction.None, MoveType.TurnToFace, FiringState.None, FiringState.None);
                return result;
                }

            public override string ToString()
                {
                var sb = new StringBuilder();
                if (this.Direction == Direction.None)
                    sb.Append("Stand still");
                else if (this.MoveType == MoveType.TurnAndMove)
                    sb.AppendFormat("Move {0}", this.Direction.ToString().ToLower());
                else
                    sb.AppendFormat("Face {0}", this.Direction.ToString().ToLower());

                if (this.FireStatus1 == FiringState.None && this.FireStatus2 == FiringState.None)
                    sb.Append(" without firing");
                else
                    {
                    if (this.FireStatus1 != FiringState.None)
                        {
                        sb.AppendFormat(", {0} fire weapon 1", this.FireStatus1.ToString().ToLower());
                        }
                    if (this.FireStatus2 != FiringState.None)
                        {
                        sb.AppendFormat(", {0} fire weapon 2", this.FireStatus2.ToString().ToLower());
                        }
                    }
                sb.Append(".");
                return sb.ToString();
                }
            }
        }

    public enum MoveType
        {
        TurnAndMove,
        TurnToFace,
        WaitingToMove
        }
    }
