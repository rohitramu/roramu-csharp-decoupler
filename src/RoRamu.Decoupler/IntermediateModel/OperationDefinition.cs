namespace RoRamu.Decoupler
{
    /// <summary>
    /// Represents the definition of an operation in a contract.
    /// </summary>
    public class OperationDefinition
    {
        /// <summary>
        /// The name of the operation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The return type of the operation.
        /// </summary>
        public string ReturnType { get; }

        /// <summary>
        /// The parameters that the operation requires as input.
        /// </summary>
        public ParameterDefinitionList Parameters { get; } = new ParameterDefinitionList();

        /// <summary>
        /// Creates a new <see cref="OperationDefinition" />.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="returnType">The return type of the operation.</param>
        public OperationDefinition(string name, string returnType)
        {
            this.Name = name;
            this.ReturnType = returnType;
        }
    }
}