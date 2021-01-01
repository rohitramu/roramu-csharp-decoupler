namespace RoRamu.Decoupler
{
    /// <summary>
    /// The definition of a contract between two components in a system.
    /// </summary>
    public class ContractDefinition
    {
        /// <summary>
        /// The name of this contract (e.g. the name of the class which will be generated from this contract).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The operations in this contract.
        /// </summary>
        public OperationDefinitionList Operations = new OperationDefinitionList();

        /// <summary>
        /// Creates a new <see cref="ContractDefinition" /> object.
        /// </summary>
        /// <param name="name">The name of this contract (e.g. the name of the class which will be generated from this contract).</param>
        public ContractDefinition(string name)
        {
            this.Name = name;
        }
    }
}