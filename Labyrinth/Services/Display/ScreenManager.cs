using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;

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

        //private readonly InputState _input = new InputState();

        private Texture2D _blankTexture;

        private bool _isInitialised;

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

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

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game) : base(game)
            {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
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

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = content.Load<SpriteFont>("menufont");
            _blankTexture = content.Load<Texture2D>("blank");

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in _screens)
                {
                screen.LoadContent();
                }
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
            }
        
        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
            {
            // Read the keyboard and gamepad.
            //_input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            var screensToUpdate = new Stack<GameScreen>(this._screens);

            bool otherScreenHasFocus = !this.Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
                {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate.Pop();

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                    {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                        {
                        screen.HandleInput(/*_input*/);

                        otherScreenHasFocus = true;
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

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
            {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in _screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
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
            if (_screens.Count > 0)
                {
                TouchPanel.EnabledGestures = _screens[_screens.Count - 1].EnabledGestures;
                }
            }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
            {
            return _screens.ToArray();
            }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
            {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin();

            SpriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            SpriteBatch.End();
            }

        /// <summary>
        /// Informs the screen manager to serialise its state to disk.
        /// </summary>
        public void SerialiseState()
            {
            return;

            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                // if our screen manager directory already exists, delete the contents
                if (storage.DirectoryExists("ScreenManager"))
                    {
                    DeleteState(storage);
                    }
                // otherwise just create the directory
                else
                    {
                    storage.CreateDirectory("ScreenManager");
                    }

                // create a file we'll use to store the list of screens in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
                    {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (GameScreen screen in _screens)
                            {
                            if (screen.IsSerializable)
                                {
                                writer.Write(screen.GetType().AssemblyQualifiedName);
                                }
                            }
                        }
                    }

                // now we create a new file stream for each screen so it can save its state
                // if it needs to. we name each file "ScreenX.dat" where X is the index of
                // the screen in the stack, to ensure the files are uniquely named
                int screenIndex = 0;
                foreach (GameScreen screen in _screens)
                    {
                    if (screen.IsSerializable)
                        {
                        string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);

                        // open up the stream and let the screen serialize whatever state it wants
                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                            {
                            screen.Serialise(stream);
                            }

                        screenIndex++;
                        }
                    }
                }
            }

        public bool DeserializeState()
            {
            return false;

            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                // see if our saved state directory exists
                if (storage.DirectoryExists("ScreenManager"))
                    {
                    try
                        {
                        // see if we have a screen list
                        if (storage.FileExists("ScreenManager\\ScreenList.dat"))
                            {
                            // load the list of screen types
                            using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read))
                                {
                                using (BinaryReader reader = new BinaryReader(stream))
                                    {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                        {
                                        // read a line from our file
                                        string line = reader.ReadString();

                                        // if it isn't blank, we can create a screen from it
                                        if (!string.IsNullOrEmpty(line))
                                            {
                                            Type screenType = Type.GetType(line);
                                            GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                                            AddScreen(screen, PlayerIndex.One);
                                            }
                                        }
                                    }
                                }
                            }

                        // next we give each screen a chance to deserialize from the disk
                        for (int i = 0; i < _screens.Count; i++)
                            {
                            string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
                            using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                                {
                                _screens[i].Deserialise(stream);
                                }
                            }

                        return true;
                        }
                    catch (Exception)
                        {
                        // if an exception was thrown while reading, odds are we cannot recover
                        // from the saved state, so we will delete it so the game can correctly
                        // launch.
                        DeleteState(storage);
                        }
                    }
                }

            return false;
            }

        /// <summary>
        /// Deletes the saved state files from isolated storage.
        /// </summary>
        private void DeleteState(IsolatedStorageFile storage)
            {
            // get all of the files in the directory and delete them
            string[] files = storage.GetFileNames("ScreenManager\\*");
            foreach (string file in files)
                {
                storage.DeleteFile(Path.Combine("ScreenManager", file));
                }
            }
        }
    }