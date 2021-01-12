namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate definitions.
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        /// Represents a function which returns a function which can execute an operation using the contract implementation.
        /// </summary>
        /// <param name="contractImplementation">The contract implementation.</param>
        /// <typeparam name="TContractImplementation">The type of the contract implementation.</typeparam>
        /// <returns>A function which can execute an operation using the given contract implementation.</returns>
        public delegate ExecuteOperationFunc GetExecuteOperationFunc<TContractImplementation>(TContractImplementation contractImplementation);

        /// <summary>
        /// Represents a function which can execute an operation.
        /// </summary>
        /// <param name="operationInvocation">The information about the operation to execute.</param>
        /// <returns>The result object if any, otherwise null.</returns>
        public delegate Task<object> ExecuteOperationFunc(OperationInvocation operationInvocation);
    }
}