namespace RoRamu.Decoupler.DotNet.Generator.Receiver
{
    using System.Threading.Tasks;

    /// <summary>
    /// Contains the behaviors for receiving an operation invocation.
    /// </summary>
    public interface IOperationInvocationReceiver
    {
        /// <summary>
        /// Receives the operation invocation so it can later be passed to an implementation of the contract.
        /// </summary>
        Task ReceiveMessageAsync(OperationInvocation operationInvocation);

        /// <summary>
        /// Receives the operation invocation so it can later be passed to an implementation of the contract.
        /// </summary>
        void ReceiveMessage(OperationInvocation operationInvocation);

        /// <summary>
        /// Receives the operation invocation so it can later be passed to an implementation of the contract.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the operation invocation.</typeparam>
        /// <returns>The result of the operation invocation.</returns>
        Task<T> ReceiveRequestAsync<T>(OperationInvocation<T> operationInvocation);

        /// <summary>
        /// Receives the operation invocation so it can later be passed to an implementation of the contract.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the operation invocation.</typeparam>
        /// <returns>The result of the operation invocation.</returns>
        T ReceiveRequest<T>(OperationInvocation<T> operationInvocation);
    }
}