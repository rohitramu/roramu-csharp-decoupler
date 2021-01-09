namespace RoRamu.Decoupler
{
    using System;

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
        public Type ReturnType { get; }

        /// <summary>
        /// A description of this operation.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The parameters that the operation requires as input.
        /// </summary>
        public ParameterDefinitionList Parameters { get; } = new ParameterDefinitionList();

        /// <summary>
        /// Creates a new <see cref="OperationDefinition" />.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="returnType">The return type of the operation.</param>
        /// <param name="description">A description of this operation.</param>
        public OperationDefinition(string name, Type returnType, string description)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Description = description;
            this.ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
        }
    }
}