namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Signals that the given operation name could not be resolved to any known operations implementations.
    /// </summary>
    [Serializable]
    public class UnknownOperationNameException : DecouplerReceiverException
    {
        /// <summary>
        /// The name of the unknown operation.
        /// </summary>
        public string OperationName { get; }

        /// <summary>
        /// Initializes a new exception.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        /// <param name="operationName">The name of the unknown operation.</param>
        internal UnknownOperationNameException(Type contractInterface, string operationName) : base(contractInterface, GetErrorMessage(contractInterface, operationName))
        {
            this.OperationName = operationName;
        }

        /// <summary>
        /// Initializes a new exception with a message and an inner exception.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        /// <param name="operationName">The name of the unknown operation.</param>
        /// <param name="innerException">The inner exception.</param>
        internal UnknownOperationNameException(Type contractInterface, string operationName, Exception innerException) : base(contractInterface, GetErrorMessage(contractInterface, operationName), innerException)
        {
            this.OperationName = operationName;
        }

        private static string GetErrorMessage(Type contractInterface, string operationName)
        {
            return $"The operation '{operationName}' could not be found in the contract/interface '{contractInterface.FullName}'.";
        }

        /// <inheritdoc/>
        protected UnknownOperationNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                info.GetValue(nameof(this.OperationName), typeof(string));
            }
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(this.OperationName), this.OperationName);
            }
        }
    }
}
