namespace RoRamu.Decoupler.DotNet.Transmitter
{
    using System;

    /// <summary>
    /// The base type for generated transmitter classes.
    /// </summary>
    public abstract class Transmitter
    {
        /// <summary>
        /// The transmitter implementation.
        /// </summary>
        public IOperationInvocationTransmitter RequestTransmitter { get; }

        /// <summary>
        /// Creates a new <see cref="Transmitter" /> object.
        /// </summary>
        /// <param name="requestTransmitter">The transmitter implementation.</param>
        protected Transmitter(IOperationInvocationTransmitter requestTransmitter)
        {
            this.RequestTransmitter = requestTransmitter ?? throw new ArgumentNullException(nameof(requestTransmitter));
        }
    }
}