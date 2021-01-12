namespace RoRamu.Decoupler.DotNet
{
    using System;
    using RoRamu.Utils.CSharp;

    /// <summary>
    /// Represents the value of a parameter provided as input to an operation.
    /// </summary>
    public class ParameterValue
    {
        /// <summary>
        /// The type of the parameter as it would be seen in fully-qualified C# code.
        /// </summary>
        public string TypeCSharpName { get; }

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
        /// <param name="typeCSharpName">The type name of the parameter as it would be seen in fully-qualified C# code.</param>
        public ParameterValue(string name, object value, string typeCSharpName = null)
        {
            this.TypeCSharpName = typeCSharpName
                ?? value?.GetType().GetCSharpName()
                ?? throw new ArgumentException("If the provided value is null, the type name must be provided.", nameof(typeCSharpName));
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

        /// <summary>
        /// Retrieves the value and casts it to the given type.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <returns>The value.</returns>
        public T GetValue<T>()
        {
            if (this.Value == null)
            {
                return default;
            }

            if (this.Value is T val)
            {
                return val;
            }

            // TODO: Make a custom exception for this
            throw new ArgumentException($"Unable to cast parameter of type '{this.Value?.GetType().GetCSharpName()}' to '{typeof(T).GetCSharpName()}'");
        }
    }
}