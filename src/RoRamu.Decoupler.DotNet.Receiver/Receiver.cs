namespace RoRamu.Decoupler.DotNet.Generator.Receiver
{
    using System;

    /// <summary>
    /// The base type for generated receiver classes.
    /// </summary>
    public abstract class Receiver
    {
        /// <summary>
        /// The receiver implementation.
        /// </summary>
        public IOperationInvocationReceiver RequestReceiver { get; }

        /// <summary>
        /// Creates a new <see cref="Receiver" /> object.
        /// </summary>
        /// <param name="requestReceiver">The receiver implementation.</param>
        public Receiver(IOperationInvocationReceiver requestReceiver)
        {
            this.RequestReceiver = requestReceiver ?? throw new ArgumentNullException(nameof(requestReceiver));
        }
    }
}