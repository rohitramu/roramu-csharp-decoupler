namespace RoRamu.Decoupler
{
    /// <summary>
    /// Creates a contract definition which can then be read by a <see cref="IContractConsumer" />.
    /// </summary>
    public interface IContractDefinitionBuilder
    {
        /// <summary>
        /// Builds the contract definition.
        /// </summary>
        ContractDefinition BuildContract();
    }
}