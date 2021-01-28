namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;

    public partial class Receiver<TContractImplementation>
    {
        /// <summary>
        /// Information about the operation implementation.
        /// </summary>
        protected class OperationImplementationInfo
        {
            /// <summary>
            /// The operation's name.
            /// </summary>
            public string OperationName { get; }

            /// <summary>
            /// The type of each parameter.
            /// </summary>
            public IEnumerable<Type> ParameterTypes { get; }

            /// <summary>
            /// The implementation of the contract.
            /// </summary>
            public Delegates.GetExecuteOperationFunc<TContractImplementation> Implementation { get; }

            /// <summary>
            /// Creates a new <see cref="OperationImplementationInfo" />.
            /// </summary>
            /// <param name="operationName">The operation's name.</param>
            /// <param name="parameterTypes">The type of each parameter.</param>
            /// <param name="implementation">The implementation of the contract.</param>
            public OperationImplementationInfo(string operationName, IEnumerable<Type> parameterTypes, Delegates.GetExecuteOperationFunc<TContractImplementation> implementation)
            {
                this.OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
                this.ParameterTypes = parameterTypes ?? Array.Empty<Type>();
                this.Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
            }
        }
    }
}