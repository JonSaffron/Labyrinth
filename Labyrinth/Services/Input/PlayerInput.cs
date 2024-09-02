using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.DataStructures;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth.Services.Input
    {
    class PlayerInput : IPlayerInput
        {
        private readonly InputState _inputState;
        private Direction _previousRequestedDirectionOfMovement;
        private static readonly List<DirectionForKey> KeysList = BuildKeys();

        public bool Enabled { get; set; }

        public PlayerInput(InputState inputState)
            {
            this._inputState = inputState ?? throw new ArgumentNullException(nameof(inputState));
            }

        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        public PlayerControl Update()
            {
            if (!this.Enabled)
                {
                return PlayerControl.NoAction;
                }

            return new PlayerControl(
                GetDirection(this._inputState),
                this._inputState.IsKeyCurrentlyPressed(Keys.LeftControl) ? (this._inputState.WasKeyPressed(Keys.LeftControl) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None,
                this._inputState.IsKeyCurrentlyPressed(Keys.Space) ? (this._inputState.WasKeyPressed(Keys.Space) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None);
            }

        private Direction GetDirection(InputState inputState)
            {
            if (GetNewDirection(inputState, out var direction))
                {
                this._previousRequestedDirectionOfMovement = direction;
                return direction;
                }

            if (GetContinuedDirection(inputState, out direction))
                {
                return direction;
                }

            direction = GetRemnantDirection(inputState);
            return direction;
            }

        private static bool GetNewDirection(InputState inputState, out Direction direction)
            {
            foreach (var item in KeysList)
                {
                if (inputState.IsKeyNewlyPressed(item.Key))
                    {
                    direction = item.Direction;
                    return true;
                    }
                }
            direction = Direction.None;
            return false;
            }

        private bool GetContinuedDirection(InputState inputState, out Direction direction)
            {
            var current = KeysList.FirstOrDefault(item => item.Direction == this._previousRequestedDirectionOfMovement);
            if (current != null)
                {
                if (inputState.IsKeyCurrentlyPressed(current.Key))
                    {
                    direction = current.Direction;
                    return true;
                    }
                }
            direction = Direction.None;
            return false;
            }

        private static Direction GetRemnantDirection(InputState inputState)
            {
            if (inputState.IsKeyCurrentlyPressed(Keys.Up) && !inputState.IsKeyCurrentlyPressed(Keys.Down))
                return Direction.Up;
            if (inputState.IsKeyCurrentlyPressed(Keys.Down) && !inputState.IsKeyCurrentlyPressed(Keys.Up))
                return Direction.Down;
            if (inputState.IsKeyCurrentlyPressed(Keys.Left) && !inputState.IsKeyCurrentlyPressed(Keys.Right))
                return Direction.Left;
            if (inputState.IsKeyCurrentlyPressed(Keys.Right) && !inputState.IsKeyCurrentlyPressed(Keys.Left))
                return Direction.Right;
            return Direction.None;
            }

        private static List<DirectionForKey> BuildKeys()
            {
            var result = new List<DirectionForKey> 
                {
                new DirectionForKey {Key = Keys.Left, Direction = Direction.Left},
                new DirectionForKey {Key = Keys.Up, Direction = Direction.Up},
                new DirectionForKey {Key = Keys.Right, Direction = Direction.Right},
                new DirectionForKey {Key = Keys.Down, Direction = Direction.Down}
                };
            return result;
            }

        private class DirectionForKey
            {
            public Keys Key;
            public Direction Direction;
            }
        }
    }
