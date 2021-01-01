namespace RoRamu.Decoupler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the list of parameters which can be provided as input to an operation.
    /// </summary>
    public class ParameterDefinitionList : IReadOnlyList<ParameterDefinition>
    {
        /// <inheritdoc />
        public ParameterDefinition this[int index] => this.ParametersInternal[index];

        /// <inheritdoc />
        public int Count => this.ParametersInternal.Count;

        private List<ParameterDefinition> ParametersInternal { get; } = new List<ParameterDefinition>();

        /// <summary>
        /// Adds a parameter definition to the end of the list.
        /// </summary>
        /// <param name="parameter">The parameter definition to add.</param>
        public void Add(ParameterDefinition parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            this.ParametersInternal.Add(parameter);
        }

        /// <inheritdoc />
        public IEnumerator<ParameterDefinition> GetEnumerator() => this.ParametersInternal.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.ParametersInternal.GetEnumerator();
    }
}