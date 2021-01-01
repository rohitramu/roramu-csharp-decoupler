namespace RoRamu.Decoupler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the list of parameters which can be provided as input to an operation.
    /// </summary>
    public class OperationDefinitionList : IReadOnlyList<OperationDefinition>
    {
        /// <inheritdoc />
        public OperationDefinition this[int index] => this.OperationsInternal[index];

        /// <inheritdoc />
        public int Count => this.OperationsInternal.Count;

        private List<OperationDefinition> OperationsInternal { get; } = new List<OperationDefinition>();

        /// <summary>
        /// Adds an operation definition to the end of the list.
        /// </summary>
        /// <param name="operation">The operation definition to add.</param>
        public void Add(OperationDefinition operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.OperationsInternal.Add(operation);
        }

        /// <inheritdoc />
        public IEnumerator<OperationDefinition> GetEnumerator() => this.OperationsInternal.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.OperationsInternal.GetEnumerator();
    }
}