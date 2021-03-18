namespace RoRamu.Decoupler.DotNet.Transmitter
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
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        Task TransmitMessageAsync(OperationInvocation operationInvocation);

        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        void TransmitMessage(OperationInvocation operationInvocation);

        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the operation invocation.</typeparam>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        /// <returns>The result of the operation invocation.</returns>
        Task<T> TransmitRequestAsync<T>(OperationInvocation operationInvocation);

        /// <summary>
        /// Transmits the operation invocation to a receiver.
        /// </summary>
        /// <typeparam name="T">The type of the value returned by the operation invocation.</typeparam>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        /// <returns>The result of the operation invocation.</returns>
        T TransmitRequest<T>(OperationInvocation operationInvocation);
    }
}