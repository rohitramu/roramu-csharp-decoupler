namespace RoRamu.Decoupler
{
    /// <summary>
    /// Consumes a contract to do something useful with it (e.g. generate a schema in a specific format or a client library).
    /// Behavior is implementation-dependent.
    /// </summary>
    public interface IContractDefinitionConsumer
    {
        /// <summary>
        /// Consumes a contract to do something useful with it (e.g. generate a schema in a specific format or a client library).
        /// </summary>
        /// <param name="contract">The contract to consume.</param>
        void Run(ContractDefinition contract);
    }
}