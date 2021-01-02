namespace RoRamu.Decoupler.DotNet.Generator.Client
{
    using System.Threading.Tasks;
    using RoRamu.Utils.Messaging;

    public interface IRequestTransmitter
    {
        Task Transmit(Message message);

        Task<RequestResult> Transmit(Request message);
    }
}