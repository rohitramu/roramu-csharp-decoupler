namespace RoRamu.Decoupler
{
    using System;
    using System.Collections.Generic;

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
        /// The full name of this contract (e.g. the fully qualified name of the class which will be generated from this contract).
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// A description of this contract.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The operations in this contract.
        /// </summary>
        public IEnumerable<OperationDefinition> Operations { get; }

        /// <summary>
        /// Creates a new <see cref="ContractDefinition" /> object.
        /// </summary>
        /// <param name="name">The name of this contract (e.g. the name of the class which will be generated from this contract).</param>
        /// <param name="fullName">The full name of this contract (e.g. the fully qualified name of the class which will be generated from this contract).</param>
        /// <param name="description">A description of this contract.</param>
        /// <param name="operations">The operations in this contract.</param>
        public ContractDefinition(string name, string fullName, string description, IEnumerable<OperationDefinition> operations)
        {
            this.Name = name;
            this.Description = description;
            this.Operations = operations ?? Array.Empty<OperationDefinition>();
            this.FullName = fullName;
        }
    }
}