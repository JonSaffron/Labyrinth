using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen
        {
        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup { get; protected set; }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime { get; protected set; } = TimeSpan.Zero;

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime { get; protected set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition { get; protected set; } = 1;

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha => 1f - TransitionPosition;

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState { get; protected set; } = ScreenState.TransitionOn;

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting { get; protected internal set; }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive => this._doesScreenHaveFocus && (this.ScreenState == ScreenState.TransitionOn || this.ScreenState == ScreenState.Active);

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager { get; internal set; }

        /// <summary>
        /// Gets the index of the player who is currently controlling this screen,
        /// or null if it is accepting input from any player. This is used to lock
        /// the game to a specific player profile. The main menu responds to input
        /// from any connected gamepad, but whichever player makes a selection from
        /// this menu is given control over all subsequent screens, so other gamepads
        /// are inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer { get; internal set; }

        private GestureType _enabledGestures = GestureType.None;

        /// <summary>
        /// Gets the gestures the screen is interested in. Screens should be as specific
        /// as possible with gestures to increase the accuracy of the gesture engine.
        /// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        /// These gestures are handled by the ScreenManager when screens change and
        /// all gestures are placed in the InputState passed to the HandleInput method.
        /// </summary>
        public GestureType EnabledGestures
            {
            get => this._enabledGestures;
            protected set
                {
                this._enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel our self.
                if (ScreenState == ScreenState.Active)
                    {
                    TouchPanel.EnabledGestures = value;
                    }
                }
            }

        /// <summary>
        /// Gets whether or not this screen is serializable. If this is true,
        /// the screen will be recorded into the screen manager's state and
        /// its Serialize and Deserialize methods will be called as appropriate.
        /// If this is false, the screen will be ignored during serialization.
        /// By default, all screens are assumed to be serializable.
        /// </summary>
        public bool IsSerializable { get; protected set; } = true;

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent()
            {
            }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent()
            {
            }

        private bool _doesScreenHaveFocus;

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool doesScreenHaveFocus, bool coveredByOtherScreen)
            {
            this._doesScreenHaveFocus = doesScreenHaveFocus;

            if (this.IsExiting)
                {
                // If the screen is going away to die, it should transition off.
                ScreenState = ScreenState.TransitionOff;

                bool isStillTransitioning = UpdateTransition(gameTime, this.TransitionOffTime, 1);
                if (!isStillTransitioning)
                    {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                    }
                }
            else if (coveredByOtherScreen)
                {
                // If the screen is covered by another, it should transition off.
                bool isStillTransitioning = UpdateTransition(gameTime, this.TransitionOffTime, 1);
                this.ScreenState = isStillTransitioning
                    ? ScreenState = ScreenState.TransitionOff
                    : ScreenState = ScreenState.Hidden;
                }
            else
                {
                // Otherwise the screen should transition on and become active.
                bool isStillTransitioning = UpdateTransition(gameTime, this.TransitionOnTime, -1);
                this.ScreenState = isStillTransitioning ? ScreenState.TransitionOn : ScreenState.Active;
                }
            }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
            {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float) (gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (this.TransitionPosition <= 0)) ||
                ((direction > 0) && (this.TransitionPosition >= 1)))
                {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
                }

            // Otherwise we are still busy transitioning.
            return true;
            }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput( /*InputState input*/)
            {
            }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
            {
            }

        /// <summary>
        /// Tells the screen to serialise its state into the given stream.
        /// </summary>
        public virtual void Serialise(Stream stream)
            {
            }

        /// <summary>
        /// Tells the screen to deserialise its state from the given stream.
        /// </summary>
        public virtual void Deserialise(Stream stream)
            {
            }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
            {
            if (this.TransitionOffTime == TimeSpan.Zero)
                {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
                }
            else
                {
                // Otherwise flag that it should transition off and then exit.
                this.IsExiting = true;
                }
            }
        }
    }
