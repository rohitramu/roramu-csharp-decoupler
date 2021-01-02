namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that the provided type does not represent an interface.
    /// </summary>
    [Serializable]
    public class NotAnInterfaceException : DecouplerGeneratorException
    {
        /// <inheritdoc/>
        public NotAnInterfaceException(Type type) : base(type, GetErrorMessage(type)) { }

        /// <inheritdoc/>
        public NotAnInterfaceException(Type type, Exception innerException) : base(type, GetErrorMessage(type), innerException) { }

        private static string GetErrorMessage(Type type)
        {
            return $"The given type is not an interface: {type.FullName}";
        }

        /// <inheritdoc/>
        protected NotAnInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
