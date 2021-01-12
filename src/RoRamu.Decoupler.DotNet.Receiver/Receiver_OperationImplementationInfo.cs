namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;

    public partial class Receiver<TContractImplementation>
    {
        protected class OperationImplementationInfo
        {
            public string OperationName { get; }

            public IEnumerable<Type> ParameterTypes { get; }

            public Delegates.GetExecuteOperationFunc<TContractImplementation> Implementation { get; }

            public OperationImplementationInfo(string operationName, IEnumerable<Type> parameterTypes, Delegates.GetExecuteOperationFunc<TContractImplementation> implementation)
            {
                this.OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
                this.ParameterTypes = parameterTypes ?? Array.Empty<Type>();
                this.Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
            }
        }
    }
}