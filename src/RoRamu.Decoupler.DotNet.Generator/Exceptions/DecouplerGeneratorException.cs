namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class for exceptions thrown by the decoupler generators.
    /// </summary>
    [Serializable]
    public abstract class DecouplerGeneratorException : Exception
    {
        /// <summary>
        /// The given type which is used as the input to the generator.
        /// </summary>
        public Type GivenType { get; }

        /// <summary>
        /// Initializes a new exception.
        /// </summary>
        /// <param name="interface">The type which was used as the input to the generator.</param>
        public DecouplerGeneratorException(Type @interface) : base()
        {
            this.GivenType = @interface;
        }

        /// <summary>
        /// Initializes a new exception with a message.
        /// </summary>
        /// <param name="interface">The type which was used as the input to the generator.</param>
        /// <param name="message">The message.</param>
        public DecouplerGeneratorException(Type @interface, string message) : base(message)
        {
            this.GivenType = @interface;
        }

        /// <summary>
        /// Initializes a new exception with a message and an inner exception.
        /// </summary>
        /// <param name="interface">The type which was used as the input to the generator.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DecouplerGeneratorException(Type @interface, string message, Exception innerException) : base(message, innerException)
        {
            this.GivenType = @interface;
        }

        /// <inheritdoc/>
        protected DecouplerGeneratorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                info.GetValue(nameof(this.GivenType), typeof(Type));
            }
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(this.GivenType), this.GivenType);
            }
        }
    }
}
