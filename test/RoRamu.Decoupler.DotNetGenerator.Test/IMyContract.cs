namespace RoRamu.Decoupler.DotNetGenerator.Test
{
    [DecouplingContract]
    public interface IMyContract
    {
        void SayHello(int @val, IMyContract test);
    }
}