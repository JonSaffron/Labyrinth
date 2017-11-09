using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth.Services.Input
    {
    class PlayerInput : IPlayerInput
        {
        public Direction Direction { get; private set; }
        public FiringState FireStatus1 { get; private set; }
        public FiringState FireStatus2 { get; private set; }

        private readonly GameInput _gameInput;
        private Direction _previousRequestedDirectionOfMovement;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        
        private static readonly List<DirectionForKey> KeysList = BuildKeys();

        public PlayerInput([NotNull] GameInput gameInput)
            {
            this._gameInput = gameInput ?? throw new ArgumentNullException(nameof(gameInput));
            }

        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        public void ProcessInput(GameTime gameTime)
            {
            this._previousKeyboardState = this._currentKeyboardState;
            this._currentKeyboardState = this._gameInput.LastKeyboardState;

            this.Direction = GetDirection();
            this.FireStatus1 = this.IsKeyCurrentlyPressed(Keys.LeftControl) ? (this.WasKeyPressed(Keys.LeftControl) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None;
            this.FireStatus2 = this.IsKeyCurrentlyPressed(Keys.Space) ? (this.WasKeyPressed(Keys.Space) ? FiringState.Continuous : FiringState.Pulse) : FiringState.None;
            }

        private Direction GetDirection()
            {
            if (GetNewDirection(out var direction))
                {
                this._previousRequestedDirectionOfMovement = direction;
                return direction;
                }

            if (GetContinuedDirection(out direction))
                {
                return direction;
                }

            direction = GetRemnantDirection();
            return direction;
            }

        private bool GetNewDirection(out Direction direction)
            {
            foreach (var item in KeysList)
                {
                if (this.IsKeyNewlyPressed(item.Key))
                    {
                    direction = item.Direction;
                    return true;
                    }
                }
            direction = Direction.None;
            return false;
            }

        private bool GetContinuedDirection(out Direction direction)
            {
            var current = KeysList.FirstOrDefault(item => item.Direction == this._previousRequestedDirectionOfMovement);
            if (current != null)
                {
                if (this.IsKeyCurrentlyPressed(current.Key))
                    {
                    direction = current.Direction;
                    return true;
                    }
                }
            direction = Direction.None;
            return false;
            }

        private Direction GetRemnantDirection()
            {
            if (this.IsKeyCurrentlyPressed(Keys.Up) && !this.IsKeyCurrentlyPressed(Keys.Down))
                return Direction.Up;
            if (this.IsKeyCurrentlyPressed(Keys.Down) && !this.IsKeyCurrentlyPressed(Keys.Up))
                return Direction.Down;
            if (this.IsKeyCurrentlyPressed(Keys.Left) && !this.IsKeyCurrentlyPressed(Keys.Right))
                return Direction.Left;
            if (this.IsKeyCurrentlyPressed(Keys.Right) && !this.IsKeyCurrentlyPressed(Keys.Left))
                return Direction.Right;
            return Direction.None;
            }

        private bool IsKeyNewlyPressed(Keys key)
            {
            bool result = this._previousKeyboardState.IsKeyUp(key) && this._currentKeyboardState.IsKeyDown(key);
            return result;
            }

        private bool IsKeyCurrentlyPressed(Keys key)
            {
            bool result = this._currentKeyboardState.IsKeyDown(key);
            return result;
            }

        private bool WasKeyPressed(Keys key)
            {
            bool result = this._previousKeyboardState.IsKeyDown(key);
            return result;
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
