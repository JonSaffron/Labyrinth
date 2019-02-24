using System;

namespace Labyrinth
    {
    /// <summary>
    /// Strongly typed property identifier for properties on a blackboard
    /// </summary>
    /// <typeparam name="T">The type of the property value it identifies</typeparam>
    public class PropertyDef<T>
        {
        /// <summary>
        /// The name of the property.
        /// </summary>
        /// <remarks>
        /// Properties are uniquely identified by name. Do not use the same name with different types.
        /// </remarks>
        public string Name { get; }

        // Factory method used to provide a default value when a propertybag
        // does not contain data for this property
        private readonly Func<T> _createDefaultValueFunc;

        /// <summary>
        /// Constructs a new property definition with a constant default value
        /// </summary>
        /// <param name="name">The name of this property</param>
        /// <param name="defaultValue">The value which will be returned if the propertybag does not contain an entry for this property.</param>
        /// <remarks>
        /// Use this constructor if the default value is a constant or a value type.
        /// </remarks>
        public PropertyDef(string name, T defaultValue = default)
            {
            this.Name = name;
            this._createDefaultValueFunc = () => defaultValue;
            }

        /// <summary>
        /// Constructs a new property definition with a default value provided by function
        /// </summary>
        /// <param name="name"></param>
        /// <param name="createDefaultValueFunc"></param>
        /// <remarks>
        /// Use this constructor if the default value is a reference type, and you
        /// do not want to share the same instance in multiple propertybags.  
        /// </remarks>
        public PropertyDef(string name, Func<T> createDefaultValueFunc)
            {
            this.Name = name;
            this._createDefaultValueFunc = createDefaultValueFunc;
            }

        public T GetDefault()
            {
            return this._createDefaultValueFunc();
            }
        }
    }
