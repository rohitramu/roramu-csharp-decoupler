namespace RoRamu.Decoupler.DotNet
{
    using System;

    /// <summary>
    /// Represents the value of a parameter provided as input to an operation.
    /// </summary>
    public class ParameterValue
    {
        /// <summary>
        /// The type of the parameter.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates a new <see cref="ParameterValue" /> instance.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>The new <see cref="ParameterValue" />.</returns>
        public static ParameterValue Create<T>(string name, T value)
        {
            return new ParameterValue(name, value, typeof(T));
        }

        /// <summary>
        /// Creates a new <see cref="ParameterValue" /> instance.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        private ParameterValue(string name, object value, Type type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Tries to retrieve the value if it is of the given type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>True if the value was successfully returned as the given type, otherwise false.</returns>
        public bool TryGetValue<T>(out T value)
        {
            if (this.Value is T val)
            {
                value = val;
                return true;
            }

            value = default;
            return false;
        }
    }
}