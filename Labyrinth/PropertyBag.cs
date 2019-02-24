using System.Collections.Generic;
using System.ComponentModel;

namespace Labyrinth
    {
    public class PropertyBag : INotifyPropertyChanged, INotifyPropertyChanging
        {
        private readonly Dictionary<string, object> _dict = new Dictionary<string, object>();

        public T Get<T>(PropertyDef<T> property)
            {
            var result = this._dict.TryGetValue(property.Name, out var returnValue)
                ? (T) returnValue
                : property.GetDefault();
            return result;
            }

        public void Set<T>(PropertyDef<T> property, T value)
            {
            OnPropertyChanging(property.Name);
            this._dict[property.Name] = value;
            OnPropertyChanged(property.Name);
            }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(string propertyName)
            {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }

        protected virtual void OnPropertyChanged(string propertyName)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
