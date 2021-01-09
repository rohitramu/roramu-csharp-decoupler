namespace RoRamu.Decoupler.DotNet.Generator.Receiver
{
    /// <summary>
    /// Generates a class which can forward calls to a given contract definition's implementation,
    /// using <see cref="Receiver" /> as the base class.
    /// </summary>
    public class ReceiverGenerator : IContractDefinitionConsumer
    {
        /// <summary>
        /// Generates the receiver class.
        /// </summary>
        /// <param name="contract">The contract for which to generate the receiver.</param>
        public void Run(ContractDefinition contract)
        {

        }
    }
}