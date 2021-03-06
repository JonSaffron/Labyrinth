﻿using System.Collections.Generic;
using System.ComponentModel;

namespace Labyrinth.DataStructures
    {
    public class PropertyBag : INotifyPropertyChanged, INotifyPropertyChanging
        {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        public T Get<T>(PropertyDef<T> property)
            {
            var result = this._dict.TryGetValue(property.Name, out var returnValue)
                ? (T) returnValue
                : property.DefaultValue;
            return result;
            }

        public void Set<T>(PropertyDef<T> property, T value)
            {
            OnPropertyChanging(property.Name);
            this._dict[property.Name] = value;
            OnPropertyChanged(property.Name);
            }

        public void Remove<T>(PropertyDef<T> property)
            {
            OnPropertyChanging(property.Name);
            this._dict.Remove(property.Name);
            OnPropertyChanged(property.Name);
            }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanging(string propertyName)
            {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }

        private void OnPropertyChanged(string propertyName)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
