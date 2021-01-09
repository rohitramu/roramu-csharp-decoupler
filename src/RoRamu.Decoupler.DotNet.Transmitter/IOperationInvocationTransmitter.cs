namespace RoRamu.Decoupler.DotNet.Generator.Transmitter
{
    using System.Threading.Tasks;

    /// <summary>
    /// Contains the behaviors for transmitting an operation invocation.
    /// </summary>
    public interface IOperationInvocationTransmitter
    {
        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        Task TransmitMessageAsync(OperationInvocation operationInvocation);

        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        void TransmitMessage(OperationInvocation operationInvocation);

        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the operation invocation.</typeparam>
        /// <returns>The result of the operation invocation.</returns>
        Task<T> TransmitRequestAsync<T>(OperationInvocation<T> operationInvocation);

        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the operation invocation.</typeparam>
        /// <returns>The result of the operation invocation.</returns>
        T TransmitRequest<T>(OperationInvocation<T> operationInvocation);
    }
}