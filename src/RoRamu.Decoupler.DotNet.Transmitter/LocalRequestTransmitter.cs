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

        public LocalRequestTransmitter(Receiver<TImplementation> receiver)
        {
            this.Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        public void TransmitMessage(OperationInvocation operationInvocation)
        {
            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.TypeCSharpName), out _);
            func(operationInvocation).GetAwaiter().GetResult();
        }

        public async Task TransmitMessageAsync(OperationInvocation operationInvocation)
        {
            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.TypeCSharpName), out _);
            await func(operationInvocation);
        }

        public T TransmitRequest<T>(OperationInvocation operationInvocation)
        {
            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.TypeCSharpName), out _);
            return (T)func(operationInvocation).GetAwaiter().GetResult();
        }

        public async Task<T> TransmitRequestAsync<T>(OperationInvocation operationInvocation)
        {
            var func = this.Receiver.GetOperationImplementation(operationInvocation.Name, operationInvocation.Parameters.Select(p => p.TypeCSharpName), out _);
            return (T)await func(operationInvocation);
        }
    }
}