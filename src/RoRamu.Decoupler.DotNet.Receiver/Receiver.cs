namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The base type for generated receiver classes.
    /// </summary>
    public abstract partial class Receiver<TContractImplementation> : IOperationInvocationReceiver
        where TContractImplementation : class
    {
        /// <summary>
        /// The implementations of operations.
        /// </summary>
        protected abstract OperationImplementationInfoCollection Operations { get; }

        /// <summary>
        /// The implementation of the contract that this class is based on.
        /// It contains the logic to execute operations.
        /// </summary>
        protected TContractImplementation ContractImplementation { get; }

        /// <summary>
        /// Creates a new <see cref="Receiver{TContractImplementation}" /> instance.
        /// </summary>
        /// <param name="contractImplementation">
        /// The implementation of the contract that this class is based on.
        /// It contains the logic to execute operations.
        /// </param>
        protected Receiver(TContractImplementation contractImplementation)
        {
            this.ContractImplementation = contractImplementation ?? throw new ArgumentNullException(nameof(contractImplementation));
        }

        /// <inheritdoc />
        public Delegates.ExecuteOperationFunc GetOperationImplementation(string operationName, IEnumerable<string> parameterTypeNames, out IEnumerable<Type> parameterTypes)
        {
            if (operationName == null)
            {
                throw new ArgumentNullException(nameof(operationName));
            }
            if (parameterTypeNames == null)
            {
                throw new ArgumentNullException(nameof(parameterTypeNames));
            }

            // Get the operation implementation (i.e. the specific overload)
            TypeNameList parameterTypeNameList = new TypeNameList(parameterTypeNames);
            OperationImplementationInfo operationImplementationInfo = this.Operations.GetOperationImplementationInfo(operationName, parameterTypeNameList);

            // Return the implementing method
            parameterTypes = operationImplementationInfo.ParameterTypes;
            Delegates.ExecuteOperationFunc implementationFunc = operationImplementationInfo.Implementation(this.ContractImplementation);
            return implementationFunc;
        }
    }
}