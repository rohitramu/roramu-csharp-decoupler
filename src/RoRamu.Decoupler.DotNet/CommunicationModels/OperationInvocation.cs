namespace RoRamu.Decoupler.DotNet
{
    using System;

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
        public ParameterValueList Parameters { get; }

        /// <summary>
        /// Creates a new <see cref="OperationInvocation" /> object.
        /// </summary>
        /// <param name="operationName">The name of the operation being invoked.</param>
        /// <param name="parameters">The parameters which will be provided as input to the operation invocation.</param>
        public OperationInvocation(string operationName, ParameterValueList parameters)
        {
            this.Name = operationName ?? throw new ArgumentNullException(nameof(operationName));
            this.Parameters = parameters ?? new ParameterValueList();
        }
    }

    /// <summary>
    /// Represents an operation invocation which returns a result.
    /// </summary>
    /// <typeparam name="TReturnType">The type of the value returned from this operation invocation.</typeparam>
    public class OperationInvocation<TReturnType> : OperationInvocation
    {
        /// <summary>
        /// The type of the objects returned by this operation.
        /// </summary>
        public Type ReturnType { get; } = typeof(TReturnType);

        /// <summary>
        /// Creates a new <see cref="OperationInvocation{TReturnType}" /> object.
        /// </summary>
        /// <param name="operationName">The name of the operation being invoked.</param>
        /// <param name="parameters">The parameters which will be provided as input to the operation invocation.</param>
        public OperationInvocation(string operationName, ParameterValueList parameters) : base(operationName, parameters)
        {

        }
    }
}