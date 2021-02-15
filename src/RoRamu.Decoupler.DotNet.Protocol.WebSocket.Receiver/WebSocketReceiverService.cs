namespace RoRamu.Decoupler.DotNet.Protocol.WebSocket.Receiver
{
    using System;

    using RoRamu.Decoupler.DotNet.Receiver;
    using RoRamu.WebSocket;
    using RoRamu.WebSocket.Server;
    using RoRamu.WebSocket.Service;

    /// <summary>
    /// A WebSocket service which can take an <see cref="OperationInvocation" /> and pass it onto
    /// a <see cref="Receiver{TContractImplementation}" />.
    /// </summary>
    public class WebSocketReceiverService<TContractImplementation> : WebSocketService<WebSocketReceiverController<TContractImplementation>>
        where TContractImplementation : class
    {
        /// <summary>
        /// The underlying receiver implementation.
        /// </summary>
        private Receiver<TContractImplementation> ReceiverImplementation { get; }

        /// <summary>
        /// Creates a new <see cref="WebSocketReceiverService{TContractImplementation}" />.
        /// </summary>
        public WebSocketReceiverService(IWebSocketServer server, Receiver<TContractImplementation> receiverImplementation) : base(server)
        {
            this.ReceiverImplementation = receiverImplementation ?? throw new ArgumentNullException(nameof(receiverImplementation));
        }

        /// <inheritdoc />
        protected override WebSocketReceiverController<TContractImplementation> CreateController(
            string connectionId,
            WebSocketConnectionInfo connectionInfo,
            IWebSocketConnection connection)
        {
            return new WebSocketReceiverController<TContractImplementation>(connectionId, connection, this.ReceiverImplementation);
        }
    }
}