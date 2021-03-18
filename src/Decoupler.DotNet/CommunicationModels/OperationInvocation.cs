namespace RoRamu.Decoupler.DotNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an operation invocation which does not return a result.
    /// </summary>
    public class OperationInvocation
    {
        /// <summary>
        /// The name of the operation being invoked.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The list of parameter values to provide as input to the operation being invoked.
        /// </summary>
        public IReadOnlyList<ParameterValue> Parameters { get; }

        /// <summary>
        /// Whether or not this operation returns anything.
        /// </summary>
        public bool HasReturnValue { get; }

        private static readonly IReadOnlyList<ParameterValue> EmptyParameterList = Array.Empty<ParameterValue>().ToList().AsReadOnly();

        /// <summary>
        /// Creates a new <see cref="OperationInvocation" /> object.
        /// </summary>
        /// <param name="name">The name of the operation being invoked.</param>
        /// <param name="parameters">The parameters which will be provided as input to the operation invocation.</param>
        /// <param name="hasReturnValue">Whether or not this operation returns anything.</param>
        public OperationInvocation(string name, IEnumerable<ParameterValue> parameters, bool hasReturnValue)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Parameters = parameters == null
                ? EmptyParameterList
                : new List<ParameterValue>(parameters).AsReadOnly();
            this.HasReturnValue = hasReturnValue;
        }
    }
}