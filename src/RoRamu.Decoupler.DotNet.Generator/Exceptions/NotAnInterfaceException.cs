namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Runtime.Serialization;
    using RoRamu.Utils.CSharp;

    /// <summary>
    /// Indicates that the provided type does not represent an interface.
    /// </summary>
    [Serializable]
    public class NotAnInterfaceException : DecouplerGeneratorException
    {
        /// <inheritdoc/>
        internal NotAnInterfaceException(Type type) : base(type, GetErrorMessage(type))
        {

        }

        /// <inheritdoc/>
        internal NotAnInterfaceException(Type type, Exception innerException) : base(type, GetErrorMessage(type), innerException)
        {

        }

        private static string GetErrorMessage(Type type)
        {
            return $"The given type is not an interface: {type.GetCSharpName()}";
        }

        /// <inheritdoc/>
        protected NotAnInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
