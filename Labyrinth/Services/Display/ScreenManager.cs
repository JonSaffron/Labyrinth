using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;
using JetBrains.Annotations;
using Labyrinth.Services.Input;

namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
        {
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly GraphicsDeviceManager _gdm;

        private Texture2D _blankTexture;
        
        private bool _isInitialised;

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public ISpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        public InputState InputState { get; } = new InputState();

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager([NotNull] Game game) : base(game)
            {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
            this._gdm = new GraphicsDeviceManager(game)
                            {
                                PreferredBackBufferWidth = (int) Constants.RoomSizeInPixels.X * Constants.ZoomWhilstWindowed,
                                PreferredBackBufferHeight = (int) Constants.RoomSizeInPixels.Y * Constants.ZoomWhilstWindowed
                            };
            var playerInput = new PlayerInput(this.InputState);
            GlobalServices.SetPlayerInput(playerInput);
            }

        /// <summary>
        /// Initialises the screen manager component.
        /// </summary>
        public override void Initialize()
            {
            base.Initialize();

            this._isInitialised = true;
            }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
            {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            this.SpriteBatch = GetSpriteBatch(this.GraphicsDevice, this._gdm.IsFullScreen);
            this.Font = content.Load<SpriteFont>("Display/MenuFont");
            this._blankTexture = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this._blankTexture.SetData(new[] { Color.White });

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in _screens)
                {
                screen.LoadContent();
                }

            GlobalServices.SoundPlayer.SoundLibrary.LoadContent(content);
            }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
            {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in _screens)
                {
                screen.UnloadContent();
                }
            this.SpriteBatch.Dispose();
            }
        
        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
            {
            // Read the keyboard and gamepad.
            this.InputState.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            var screensToUpdate = new Stack<GameScreen>(this._screens);

            bool doesScreenHaveFocus = this.Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
                {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate.Pop();

                // Update the screen.
                screen.Update(gameTime, doesScreenHaveFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                    {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (doesScreenHaveFocus)
                        {
                        screen.HandleInput(/*_input*/);

                        doesScreenHaveFocus = false;
                        }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                    }
                }

            // Print debug trace?
            if (TraceEnabled)
                TraceScreens();
            }

        private string _lastListOfScreens;

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
            {
            var listOfScreens = string.Join(", ", this._screens.Select(item => item.GetType().Name));
            if (listOfScreens == _lastListOfScreens)
                return;
            Debug.WriteLine(listOfScreens);
            this._lastListOfScreens = listOfScreens;
            }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
            {
            foreach (GameScreen screen in _screens)
                {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
                }
            }
        
        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
            {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (this._isInitialised)
                {
                screen.LoadContent();
                }

            this._screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
            }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
            {
            // If we have a graphics device, tell the screen to unload content.
            if (this._isInitialised)
                {
                screen.UnloadContent();
                }

            this._screens.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (this._screens.Count > 0)
                {
                TouchPanel.EnabledGestures = _screens[_screens.Count - 1].EnabledGestures;
                }
            }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public IEnumerable<GameScreen> Screens => this._screens;

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
            {
            Viewport viewport = GraphicsDevice.Viewport;
            var r = new Rectangle(0, 0, viewport.Width, viewport.Height);

            this.SpriteBatch.Begin(Vector2.Zero);
            SpriteBatch.DrawRectangle(r, Color.Black * alpha);
            SpriteBatch.End();
            }

        public void ToggleFullScreen()
            {
            this._gdm.ToggleFullScreen();
            this.SpriteBatch = GetSpriteBatch(this.GraphicsDevice, this._gdm.IsFullScreen);
            }

        private static ISpriteBatch GetSpriteBatch(GraphicsDevice graphicsDevice, bool isFullScreen)
            {
            var result = isFullScreen ? (ISpriteBatch) new SpriteBatchFullScreen(graphicsDevice) : new SpriteBatchWindowed(graphicsDevice, Constants.ZoomWhilstWindowed);
            return result;
            }

        }
    }
