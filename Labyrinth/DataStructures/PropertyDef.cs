using System;

namespace Labyrinth.DataStructures
    {
    /// <summary>
    /// Strongly typed property identifier for properties
    /// </summary>
    /// <typeparam name="T">The type of the property value it identifies</typeparam>
    public class PropertyDef<T>
        {
        /// <summary>
        /// The name of the property
        /// </summary>
        /// <remarks>
        /// Properties are uniquely identified by name. Do not use the same name with different types.
        /// </remarks>
        public string Name { get; }

        // Factory method used to provide a default value when a PropertyBag
        // does not contain data for this property
        private readonly Func<T> _createDefaultValueFunc;

        /// <summary>
        /// Constructs a new property definition with a constant default value
        /// </summary>
        /// <param name="name">The name of this property</param>
        /// <param name="defaultValue">The value which will be returned if the PropertyBag does not contain an entry for this property.</param>
        /// <remarks>
        /// Use this constructor if the default value is a constant or a value type.
        /// </remarks>
        public PropertyDef(string name, T defaultValue)
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
        /// do not want to share the same instance in multiple PropertyBags.  
        /// </remarks>
        public PropertyDef(string name, Func<T> createDefaultValueFunc)
            {
            this.Name = name;
            this._createDefaultValueFunc = createDefaultValueFunc;
            }

        /// <summary>
        /// Gets or generates the default value for this property
        /// </summary>
        /// <returns>A value that represents the default value for this property</returns>
        public T DefaultValue => this._createDefaultValueFunc();
        }
    }
