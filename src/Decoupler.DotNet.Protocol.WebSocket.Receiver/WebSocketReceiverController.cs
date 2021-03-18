namespace RoRamu.Decoupler.DotNet.Protocol.WebSocket.Receiver
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RoRamu.Decoupler.DotNet.Receiver;
    using RoRamu.Utils.Messaging;
    using RoRamu.WebSocket;
    using RoRamu.WebSocket.Service;

    /// <summary>
    /// The controller which passes an <see cref="OperationInvocation" /> to a
    /// <see cref="Receiver{TContractImplementation}" />.
    /// </summary>
    /// <typeparam name="TContractImplementation">
    /// The type of the interface that the given <see cref="Receiver{TContractImplementation}" />
    /// implements.
    /// </typeparam>
    public class WebSocketReceiverController<TContractImplementation> : WebSocketServiceController
        where TContractImplementation : class
    {
        private IMessageHandlerCollection MessageHandlerCollection { get; }

        private Receiver<TContractImplementation> ReceiverImplementation { get; }

        /// <summary>
        /// Creates a new <see cref="WebSocketReceiverController{TContractImplementation}" />.
        /// </summary>
        public WebSocketReceiverController(
            string connectionId,
            IWebSocketConnection webSocketConnection,
            Receiver<TContractImplementation> receiverImplementation)
            : base(connectionId, webSocketConnection)
        {
            this.ReceiverImplementation = receiverImplementation ?? throw new ArgumentNullException(nameof(receiverImplementation));

            this.MessageHandlerCollection = MessageHandlerCollectionBuilder
                .Create()
                .SetHandler(Constants.MessageType, this.InvokeOperationWithMessage)
                .Build();
        }

        /// <inheritdoc />
        public override async Task OnMessage(Message message)
        {
            await this.MessageHandlerCollection.HandleMessage(message);
        }

        private async Task InvokeOperationWithMessage(Message message)
        {
            OperationInvocation op = message.GetBody<OperationInvocation>();
            var invokeFunc = this.ReceiverImplementation.GetOperationImplementation(op, out IEnumerable<Type> parameterTypes);
            object invocationResult = await invokeFunc(op);

            if (message.IsRequest())
            {
                Response response = message.CreateResponse(invocationResult);
                await this.Connection.SendMessage(response);
            }
        }
    }
}