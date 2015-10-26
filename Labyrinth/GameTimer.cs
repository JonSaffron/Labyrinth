﻿using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public class GameTimer : IGameComponent, IUpdateable
        {
        public TimeSpan TimeRemaining { get; private set; }

        private readonly EventHandler _callBack;
        private bool _enabled;
        private int _updateOrder;
        private readonly GameComponentCollection _parentCollection;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public static GameTimer AddGameTimer(TimeSpan timeToElapse, EventHandler callBack, bool isEnabled = true)
            {
            var gameComponentCollection = GlobalServices.GameComponentCollection;
            var gt = new GameTimer(gameComponentCollection, timeToElapse, callBack, isEnabled);
            gameComponentCollection.Add(gt);
            return gt;
            }

        private GameTimer(GameComponentCollection parentCollection, TimeSpan timeToElapse, EventHandler callBack, bool isEnabled = true)
            {
            if (timeToElapse.Ticks < 0)
                throw new ArgumentOutOfRangeException("timeToElapse");
            if (callBack == null)
                throw new ArgumentNullException("callBack");

            this.TimeRemaining = timeToElapse;
            this._callBack = callBack;
            this.Enabled = isEnabled;
            this.UpdateOrder = 0;
            this._parentCollection = parentCollection;
            }

        public void Update(GameTime gameTime)
            {
            this.TimeRemaining -= gameTime.ElapsedGameTime;
            if (TimeRemaining.Ticks <= 0)
                {
                this._parentCollection.Remove(this);
                this._callBack(this, new EventArgs());
                }
            }

        public void Initialize()
            {
            // nothing to do
            }

        public bool Enabled
            {
            get
                {
                return this._enabled;
                }
            set
                {
                if (this._enabled != value)
                    {
                    this._enabled = value;
                    this.OnEnabledChanged();
                    }
                }
            }

        private void OnEnabledChanged()
            {
            var handler = this.EnabledChanged;
            if (handler != null)
                {
                handler(this, new EventArgs());
                }
            }

        public int UpdateOrder
            {
            get 
                {
                return this._updateOrder;
                }
            set
                {
                if (this.UpdateOrder != value)
                    {
                    this._updateOrder = value;
                    this.OnUpdateOrderChanged();
                    }
                }
            }

        private void OnUpdateOrderChanged()
            {
            var handler = this.UpdateOrderChanged;
            if (handler != null)
                {
                handler(this, new EventArgs());
                }
            }

        }
    }
