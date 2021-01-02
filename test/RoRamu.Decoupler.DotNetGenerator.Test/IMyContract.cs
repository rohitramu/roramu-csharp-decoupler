namespace RoRamu.Decoupler.DotNet.Generator.Test
{
    [DecouplingContract]
    public interface IMyContract
    {
        void SayHello(int @val, IMyContract test);

        int SayGoodbye(string message);
    }
}