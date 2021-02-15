namespace RoRamu.Decoupler.DotNet.Protocol.WebSocket.Transmitter
{
    using System;
    using System.Threading.Tasks;

    using RoRamu.Decoupler.DotNet;
    using RoRamu.Utils.Messaging;
    using RoRamu.WebSocket.Client;
    using RoRamu.Decoupler.DotNet.Transmitter;

    /// <summary>
    /// A <see cref="Transmitter" /> implementation using the WebSocket protocol.
    /// </summary>
    public class WebSocketTransmitter : IOperationInvocationTransmitter
    {
        /// <summary>
        /// The underlying WebSocket connection.
        /// </summary>
        public WebSocketClient Client { get; }

        /// <summary>
        /// True if the WebSocket client's connection is open, otherwise false.
        /// </summary>
        public bool IsConnected => this.Client.IsOpen();

        /// <summary>
        /// Creates a new <see cref="WebSocketTransmitter" />.
        /// </summary>
        /// <param name="client">The WebSocket client to use.</param>
        public WebSocketTransmitter(WebSocketClient client)
        {
            this.Client = client ?? throw new ArgumentNullException();
        }

        /// <inheritdoc />
        public void TransmitMessage(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            this.TransmitMessageAsync(operationInvocation).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task TransmitMessageAsync(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            await this.Client.SendMessage(new Message(null, Constants.MessageType, operationInvocation));
        }

        /// <inheritdoc />
        public T TransmitRequest<T>(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            return this.TransmitRequestAsync<T>(operationInvocation).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task<T> TransmitRequestAsync<T>(OperationInvocation operationInvocation)
        {
            if (operationInvocation == null)
            {
                throw new ArgumentNullException(nameof(operationInvocation));
            }

            // Create the request
            Request request = new Request(Constants.MessageType, operationInvocation);

            // Send the request
            RequestResult result = await this.Client.SendRequest(request);

            // If the request failed, throw an exception
            if (!result.IsSuccessful)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                else
                {
                    //TODO: Make a custom exception
                    throw new Exception($"Request failed: {request.Id}");
                }
            }

            // Return the result
            return result.Response.GetBody<T>();
        }
    }
}