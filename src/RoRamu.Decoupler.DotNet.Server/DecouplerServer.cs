namespace RoRamu.Decoupler.DotNet.Generator.Server
{
    using System;

    public abstract class DecouplerServer
    {
        public IRequestReceiver RequestReceiver { get; }

        public DecouplerServer(IRequestReceiver requestReceiver)
        {
            this.RequestReceiver = requestReceiver ?? throw new ArgumentNullException(nameof(requestReceiver));
        }
    }
}