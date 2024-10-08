﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Behaviour
    {
    public class BehaviourCollection : Collection<IBehaviour>
        {
        private readonly Monster _monster;

        public BehaviourCollection(Monster monster)
            {
            this._monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public void Perform<T>() where T : IBehaviour
            {
            var listOfActions = this.Items.OfType<T>().ToArray();
            foreach (var action in listOfActions)
                action.Perform();
            }

        [PublicAPI]
        public bool Has<T>() where T : IBehaviour
            {
            bool result = this.Items.OfType<T>().Any();
            return result;
            }

        public void Add<T>() where T : BaseBehaviour, new()
            {
            if (this.Has<T>())
                return;
            bool hasSpecificInterface = typeof(T).FindInterfaces(TypeFilter, null).Any();
            if (!hasSpecificInterface)
                throw new InvalidOperationException($"Behaviour {typeof(T).Name} does not have a specific behaviour (movement, injury, death)");
            var newBehaviour = new T();
            newBehaviour.Init(this._monster);
            base.Add(newBehaviour);
            }

        private static bool TypeFilter(Type m, object? filterCriteria)
            {
            return m == typeof(IInjuryBehaviour) || m == typeof(IMovementBehaviour) || m == typeof(IDeathBehaviour);
            }

        public void Remove<T>() where T : IBehaviour
            {
            for (int i = this.Items.Count - 1; i >= 0; i--)
                {
                if (this.Items[i] is T)
                    {
                    this.Items.RemoveAt(i);
                    break;
                    }
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
