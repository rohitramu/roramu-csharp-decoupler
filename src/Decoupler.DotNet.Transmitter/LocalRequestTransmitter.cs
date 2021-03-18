namespace RoRamu.Decoupler.DotNet.Transmitter
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using RoRamu.Decoupler.DotNet.Receiver;

    /// <summary>
    /// An implementation of a <see cref="IOperationInvocationTransmitter" /> which directly calls a
    /// local instance of a receiver.
    /// </summary>
    /// <typeparam name="TImplementation">The interface from which the transmitter was generated.</typeparam>
    public class LocalRequestTransmitter<TImplementation> : IOperationInvocationTransmitter
        where TImplementation : class
    {
        private Receiver<TImplementation> Receiver { get; }

        /// <summary>
        /// Creates a new <see cref="LocalRequestTransmitter{TImplementation}" />.
        /// </summary>
        /// <param name="receiver">The new <see cref="LocalRequestTransmitter{TImplementation}" />.</param>
        public LocalRequestTransmitter(Receiver<TImplementation> receiver)
        {
            this.Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        /// <summary>
        /// Transmits an operation invocation which represents a message.
        /// </summary>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        public void TransmitMessage(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.CSharpTypeName), out _);
            func(operationInvocation).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Transmits an operation invocation which represents a message asynchronously.
        /// </summary>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        public async Task TransmitMessageAsync(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.CSharpTypeName), out _);
            await func(operationInvocation);
        }

        /// <summary>
        /// Transmits an operation invocation which represents a request.
        /// </summary>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        /// <returns>The result of the request.</returns>
        public T TransmitRequest<T>(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.CSharpTypeName), out _);
            return (T)func(operationInvocation).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Transmits an operation invocation which represents a request asynchronously.
        /// </summary>
        /// <param name="operationInvocation">The operation invocation to transmit.</param>
        /// <returns>The result of the request.</returns>
        public async Task<T> TransmitRequestAsync<T>(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.CSharpTypeName), out _);
            return (T)await func(operationInvocation);
        }
    }
}