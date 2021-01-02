namespace RoRamu.Decoupler.DotNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ParameterList : IReadOnlyList<ParameterValue>
    {
        public ParameterValue this[int index] => this.ParametersInternal[index];

        public int Count => this.ParametersInternal.Count;

        private List<ParameterValue> ParametersInternal { get; } = new List<ParameterValue>();

        public void Add(ParameterValue parameterValue)
        {
            this.ParametersInternal.Add(parameterValue ?? throw new ArgumentNullException(nameof(parameterValue)));
        }

        public IEnumerator<ParameterValue> GetEnumerator() => this.ParametersInternal.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.ParametersInternal.GetEnumerator();
    }
}