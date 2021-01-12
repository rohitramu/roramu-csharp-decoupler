namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class for exceptions thrown by receiver implementations.
    /// </summary>
    [Serializable]
    public abstract class DecouplerReceiverException : Exception
    {
        /// <summary>
        /// The interface type which the receiver implementation was generated from.
        /// </summary>
        public Type ContractInterface { get; }

        /// <summary>
        /// Initializes a new exception.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        public DecouplerReceiverException(Type contractInterface) : base()
        {
            this.ContractInterface = contractInterface;
        }

        /// <summary>
        /// Initializes a new exception with a message.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        /// <param name="message">The message.</param>
        public DecouplerReceiverException(Type contractInterface, string message) : base(message)
        {
            this.ContractInterface = contractInterface;
        }

        /// <summary>
        /// Initializes a new exception with a message and an inner exception.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DecouplerReceiverException(Type contractInterface, string message, Exception innerException) : base(message, innerException)
        {
            this.ContractInterface = contractInterface;
        }

        /// <inheritdoc/>
        protected DecouplerReceiverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                info.GetValue(nameof(this.ContractInterface), typeof(Type));
            }
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(this.ContractInterface), this.ContractInterface);
            }
        }
    }
}
