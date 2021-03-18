namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Signals that the given parameter list could not be resolved to any known operations' overloads.
    /// </summary>
    [Serializable]
    public class UnknownOperationOverloadException : UnknownOperationNameException
    {
        /// <summary>
        /// The list of parameters' type names.
        /// </summary>
        public IEnumerable<string> ParameterTypeNames { get; }

        /// <summary>
        /// Initializes a new exception.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        /// <param name="operationName">The name of the operation for which the requested overload could not be found.</param>
        /// <param name="parameterTypeNames">The list of parameters' type names.</param>
        internal UnknownOperationOverloadException(Type contractInterface, string operationName, IEnumerable<string> parameterTypeNames) : base(contractInterface, GetErrorMessage(contractInterface, operationName, parameterTypeNames))
        {
            this.ParameterTypeNames = parameterTypeNames;
        }

        /// <summary>
        /// Initializes a new exception with a message and an inner exception.
        /// </summary>
        /// <param name="contractInterface">The interface type which the receiver implementation was generated from.</param>
        /// <param name="operationName">The name of the operation for which the requested overload could not be found.</param>
        /// <param name="parameterTypeNames">The list of parameters' type names.</param>
        /// <param name="innerException">The inner exception.</param>
        internal UnknownOperationOverloadException(Type contractInterface, string operationName, IEnumerable<string> parameterTypeNames, Exception innerException) : base(contractInterface, GetErrorMessage(contractInterface, operationName, parameterTypeNames), innerException)
        {
            this.ParameterTypeNames = parameterTypeNames;
        }

        private static string GetErrorMessage(Type contractInterface, string operationName, IEnumerable<string> parameterTypeNames)
        {
            return $"A specific overload for the operation '{operationName}' in the contract/interface '{contractInterface.FullName}' could not be found with the following parameter type list: {string.Join(", ", parameterTypeNames)}";
        }

        /// <inheritdoc/>
        protected UnknownOperationOverloadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                info.GetValue(nameof(this.ParameterTypeNames), typeof(IEnumerable<string>));
            }
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(this.ParameterTypeNames), this.ParameterTypeNames);
            }
        }
    }
}
