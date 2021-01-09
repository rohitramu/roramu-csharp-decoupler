namespace RoRamu.Decoupler.DotNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of parameters provided as input to an operation.
    /// </summary>
    public class ParameterValueList : IReadOnlyList<ParameterValue>
    {
        /// <inheritdoc />
        public ParameterValue this[int index] => this.ParametersInternal[index];

        /// <inheritdoc />
        public int Count => this.ParametersInternal.Count;

        private List<ParameterValue> ParametersInternal { get; } = new List<ParameterValue>();

        /// <summary>
        /// Creates a new <see cref="ParameterValueList" />.
        /// </summary>
        /// <param name="parameters">The parameters in the list</param>
        public ParameterValueList(IEnumerable<ParameterValue> parameters = null)
        {
            if (parameters != null)
            {
                this.ParametersInternal.AddRange(parameters);
            }
        }

        /// <inheritdoc />
        public IEnumerator<ParameterValue> GetEnumerator() => this.ParametersInternal.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.ParametersInternal.GetEnumerator();
    }
}