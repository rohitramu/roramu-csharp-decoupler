namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that the provided type does not represent an interface.
    /// </summary>
    [Serializable]
    public class InvalidParameterInInterfaceMethodException : InvalidMemberInInterfaceException
    {
        /// <summary>
        /// The invalid member in the provided interface.
        /// </summary>
        public ParameterInfo InvalidParameter { get; }

        /// <inheritdoc/>
        internal InvalidParameterInInterfaceMethodException(Type @interface, MethodInfo method, ParameterInfo invalidParameter, string reason) : base(
            @interface,
            method,
            GetErrorMessage(invalidParameter, reason))
        {
            this.InvalidParameter = invalidParameter;
        }

        /// <inheritdoc/>
        internal InvalidParameterInInterfaceMethodException(Type @interface, MethodInfo method, ParameterInfo invalidParameter, string reason, Exception innerException) : base(
            @interface,
            method,
            GetErrorMessage(invalidParameter, reason), innerException)
        {
            this.InvalidParameter = invalidParameter;
        }

        private static string GetErrorMessage(ParameterInfo invalidParameter, string reason)
        {
            return $"The parameter '{invalidParameter.Name}' is not allowed: {reason}";
        }

        /// <inheritdoc/>
        protected InvalidParameterInInterfaceMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                info.GetValue(nameof(this.InvalidParameter), typeof(ParameterInfo));
            }
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(this.InvalidParameter), this.InvalidParameter);
            }
        }
    }
}
