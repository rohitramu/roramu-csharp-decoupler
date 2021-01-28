namespace RoRamu.Decoupler.DotNet.Protocol.WebSocket.Transmitter
{
    using RoRamu.WebSocket.Client;

    /// <summary>
    /// A transmitter implementation using the WebSocket protocol.
    /// </summary>
    /// <typeparam name="TContractType">The interface type for the contract.</typeparam>
    public class DecouplerWebSocketTransmitter<TContractType>
    {
        /// <summary>
        /// Creates a new <see cref="DecouplerWebSocketTransmitter{TContractType}" />.
        /// </summary>
        /// <param name="connection">The underlying WebSocket connection to use.</param>
        public DecouplerWebSocketTransmitter(WebSocketClientConnection connection)
        {

        }
    }
}