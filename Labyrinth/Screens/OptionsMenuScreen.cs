using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.DataStructures;
using Labyrinth.Services;
using Labyrinth.Services.Sound;

namespace Labyrinth.Screens
    {
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    internal class OptionsMenuScreen : MenuScreen
        {
        private readonly MenuEntry _worldMenuEntry;
        private readonly MenuEntry _volumeOnMenuEntry;
        private readonly MenuEntry _screenModeEntry;

        private readonly List<WorldInfo> _worldList;
        private int _selectedWorld;

        /// <summary>
        /// Constructs a new instance of the options menu screen
        /// </summary>
        public OptionsMenuScreen() : base("Options")
            {
            this._worldList = WorldManagement.EnumerateAvailableWorlds().OrderBy(item => item.Name).ToList();
            this._selectedWorld = 1;
            var savedWorld = PersistentStorage.GetWorld();
            if (savedWorld != null)
                {
                var worldInfo = this._worldList.SingleOrDefault(item => string.Equals(item.File, savedWorld, StringComparison.OrdinalIgnoreCase));
                if (worldInfo != null)
                    {
                    this._selectedWorld = this._worldList.IndexOf(worldInfo);
                    }
                }

            // Create our menu entries.
            _worldMenuEntry = new MenuEntry(string.Empty);
            _volumeOnMenuEntry = new MenuEntry(string.Empty);
            this._screenModeEntry = new MenuEntry(string.Empty);

            //SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            _worldMenuEntry.Selected += WorldMenuEntrySelected;
            _volumeOnMenuEntry.Selected += VolumeOnMenuEntrySelected;
            this._screenModeEntry.Selected += ScreenModeEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(_worldMenuEntry);
            MenuEntries.Add(_volumeOnMenuEntry);
            MenuEntries.Add(this._screenModeEntry);
            MenuEntries.Add(back);
            }

        public override void Activate(bool instancePreserved = false)
            {
            base.Activate(instancePreserved);

            SetMenuEntryText();
            }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        private void SetMenuEntryText()
            {
            _worldMenuEntry.Text = "World: " + this._worldList[this._selectedWorld].Name;
            _volumeOnMenuEntry.Text = "Volume: " + (VolumeControl.Instance.IsMuted ? "Muted" : "On");
            this._screenModeEntry.Text = "Change to: " + (ScreenManager.IsFullScreen ? "Windowed mode" : "Full screen mode");
            }

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        private void WorldMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
            {
            this._selectedWorld = (this._selectedWorld + 1) % this._worldList.Count;
            PersistentStorage.SetWorld(this._worldList[this._selectedWorld].File);

            SetMenuEntryText();
            }

        private void VolumeOnMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
            {
            if (VolumeControl.Instance.IsMuted)
                {
                VolumeControl.Instance.Unmute();
                }
            else
                {
                VolumeControl.Instance.Mute();
                }

            SetMenuEntryText();
            }

        private void ScreenModeEntrySelected(object? sender, PlayerIndexEventArgs args)
            {
            this.ScreenManager.ToggleFullScreen();
            SetMenuEntryText();
            }
        }
    }
