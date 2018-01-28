﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Actions
    {
    public class BehaviourCollection : Collection<IBehaviour>
        {
        private readonly Monster _monster;

        public BehaviourCollection([NotNull] Monster monster)
            {
            _monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public void PerformAll()
            {
            foreach (var action in this.Items)
                action.Perform();
            }

        public bool Has<T>() where T : IBehaviour
            {
            bool result = this.Items.OfType<T>().Any();
            return result;
            }

        public void Add<T>() where T : BaseBehaviour, new() 
            {
            if (!this.Has<T>())
                return;
            var newBehaviour = new T();
            newBehaviour.Init(this._monster);
            base.Add(newBehaviour);
            }

        public void Remove<T>() where T : IBehaviour
            {
            for (int i = this.Items.Count - 1; i >= 0; i--)
                {
                if (this.Items[i] is T)
                    this.Items.RemoveAt(i);
                }
            }

        public void Set<T>(bool include) where T : BaseBehaviour, new()
            {
            if (include)
                {
                Add<T>();
                }
            else
                {
                Remove<T>();
                }
            }

        protected override void InsertItem(int index, IBehaviour newItem)
            {
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));
            base.InsertItem(index, newItem);
            }

        protected override void SetItem(int index, IBehaviour item)
            {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            base.SetItem(index, item);
            }
        }
    }