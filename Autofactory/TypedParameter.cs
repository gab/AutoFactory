using System;

namespace AutoFactory
{
    /// <summary>
    /// Class representing a constructor parameter with the type and the value.
    /// </summary>
    public class TypedParameter
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; private set; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedParameter"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public TypedParameter(Type type, object value) 
        { 
            Type = type;
            Value = value;
        }
        /// <summary>
        /// Creates a typed parameter from the type and the value
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="value">The value.</param>
        public static TypedParameter From<T>(T value) 
        { 
            return new TypedParameter(typeof(T), value); 
        } 
    }
}