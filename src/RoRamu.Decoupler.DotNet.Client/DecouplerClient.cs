namespace RoRamu.Decoupler.DotNet.Generator.Client
{
    using System;

    public abstract class DecouplerClient
    {
        public IRequestTransmitter RequestTransmitter { get; }

        public DecouplerClient(IRequestTransmitter requestTransmitter)
        {
            this.RequestTransmitter = requestTransmitter ?? throw new ArgumentNullException(nameof(requestTransmitter));
        }
    }
}