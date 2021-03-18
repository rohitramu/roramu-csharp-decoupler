namespace RoRamu.Decoupler.DotNet.Generator.Test
{
    using System.Threading.Tasks;

    /// <summary>
    /// This is my contract.
    /// </summary>
    [DecouplingContract]
    public interface IMyContract
    {
        /// <summary>
        /// Says hello.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="test">A test parameter.</param>
        void SayHello(long @val, IMyContract test);

        /// <summary>
        /// Says goodbye with a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An integer.</returns>
        string SayGoodbye(string message);

        /// <summary>
        /// An asynchronous goodbye.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An integer.</returns>
        Task<string> SayGoodbyeAsync(string message);

        /// <summary>
        /// Says nothing.  A no-op.
        /// </summary>
        void SayNothing();

        /// <summary>
        /// Says nothing asynchronously.  A no-op.
        /// </summary>
        Task SayNothingAsync();
    }
}