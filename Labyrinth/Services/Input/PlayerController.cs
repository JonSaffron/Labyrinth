using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Labyrinth.Test
    {
    class PlayerController : IPlayerInput
        {
        public GameInput GameInput { get; set; }
        public Direction Direction { get; private set; }
        public FiringState FireStatus1 { get; private set; }
        public FiringState FireStatus2 { get; private set; }
        
        private readonly Queue<TimedInstruction> _instructions = new Queue<TimedInstruction>(); 
        private Instruction _currentInstruction = Instruction.DoNothingInstruction();
        private TimedInstruction _nextInstruction = TimedInstruction.InitialState();

        public PlayerController()
            {
            }

        public PlayerController(TimedInstruction instruction)
            {
            if (instruction != null)
                {
                this._nextInstruction = instruction;
                }
            }

        public PlayerController(IEnumerable<TimedInstruction> instructions)
            {
            if (instructions != null)
                {
                foreach (var item in instructions)
                    {
                    this._instructions.Enqueue(item);
                    }
                this._nextInstruction = this._instructions.Dequeue();
                }
            }

        public void ProcessInput(GameTime gameTime)
            {
            if (this._nextInstruction != null && gameTime.TotalGameTime >= this._nextInstruction.TakesEffectFrom)
                {
                this._currentInstruction = this._nextInstruction.Instruction;
                System.Diagnostics.Trace.WriteLine("ProcessInput changing to: " + this._currentInstruction);
                this._nextInstruction = this._instructions.Count != 0 ? this._instructions.Dequeue() : null;
                }

            this.Direction = this._currentInstruction.Direction;
            this.FireStatus1 = this._currentInstruction.FireStatus1;
            this.FireStatus2 = this._currentInstruction.FireStatus2;
            this._currentInstruction = Instruction.DoNothingInstruction();
            }

        public bool HasFinishedQueue
            {
            get
                {
                var result = this._nextInstruction == null;
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

            public static TimedInstruction InitialState()
                {
                var result = new TimedInstruction(TimeSpan.Zero, Instruction.DoNothingInstruction());
                return result;
                }

            public override string ToString()
                {
                var result = string.Format("At {0}, {1}", this.TakesEffectFrom, this.Instruction);
                return result;
                }
            }

        public class Instruction
            {
            public readonly Direction Direction;
            public readonly FiringState FireStatus1;
            public readonly FiringState FireStatus2;

            public Instruction(Direction direction, FiringState fireStatus1, FiringState fireStatus2)
                {
                this.Direction = direction;
                this.FireStatus1 = fireStatus1;
                this.FireStatus2 = fireStatus2;
                }

            public static Instruction DoNothingInstruction()
                {
                var result = new Instruction(Direction.None, FiringState.None, FiringState.None);
                return result;
                }

            public override string ToString()
                {
                var sb = new StringBuilder();
                if (this.Direction == Direction.None)
                    sb.Append("Stand still");
                else
                    sb.AppendFormat("Move {0}", this.Direction.ToString().ToLower());
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
    }
