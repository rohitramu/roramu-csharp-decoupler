namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class Receiver<TContractImplementation>
    {
        /// <summary>
        /// A collection of
        /// </summary>
        protected class OperationImplementationInfoCollection
        {
            private IDictionary<string, IDictionary<TypeNameList, OperationImplementationInfo>> OperationImplementationInfos { get; } = new Dictionary<string, IDictionary<TypeNameList, OperationImplementationInfo>>();

            /// <summary>
            /// Adds information about an operation implementation to this collection.
            /// </summary>
            /// <param name="operationName">The operation's name.</param>
            /// <param name="parameterTypes">The operation's parameter types (order matters).</param>
            /// <param name="getImplementationFunc">A method to get the operation's implementation.</param>
            public void Add(string operationName, IEnumerable<Type> parameterTypes, Delegates.GetExecuteOperationFunc<TContractImplementation> getImplementationFunc)
            {
                if (operationName == null)
                {
                    throw new ArgumentNullException(nameof(operationName));
                }
                if (parameterTypes == null)
                {
                    throw new ArgumentNullException(nameof(parameterTypes));
                }
                if (getImplementationFunc == null)
                {
                    throw new ArgumentNullException(nameof(getImplementationFunc));
                }

                // Add an entry for the operation name if it doesn't exist
                if (!this.OperationImplementationInfos.ContainsKey(operationName))
                {
                    this.OperationImplementationInfos.Add(operationName, new Dictionary<TypeNameList, OperationImplementationInfo>());
                }

                // Get the operation overloads
                IDictionary<TypeNameList, OperationImplementationInfo> overloads = this.OperationImplementationInfos[operationName];

                // Make sure that the overload isn't already defined
                TypeNameList parameterTypeNames = new TypeNameList(parameterTypes); // Should match ParameterValue.TypeCSharpName
                if (overloads.ContainsKey(parameterTypeNames))
                {
                    throw new ArgumentException($"In operation '{operationName}', an overload with the following parameters is already defined: {parameterTypes}");
                }

                // Store the implementation info
                OperationImplementationInfo implementationInfo = new OperationImplementationInfo(operationName, parameterTypes, getImplementationFunc);
                overloads.Add(parameterTypeNames, implementationInfo);
            }

            /// <summary>
            /// Gets an <see cref="OperationImplementationInfo" /> object from this collection.
            /// </summary>
            /// <param name="operationName">The operation name.</param>
            /// <param name="parameterTypeNames">The names of the parameter types (in the correct order).</param>
            /// <returns>The <see cref="OperationImplementationInfo" />.</returns>
            public OperationImplementationInfo GetOperationImplementationInfo(string operationName, IEnumerable<string> parameterTypeNames)
            {
                if (!this.OperationImplementationInfos.TryGetValue(operationName, out IDictionary<TypeNameList, OperationImplementationInfo> operationImplementationInfos))
                {
                    throw new UnknownOperationNameException(typeof(TContractImplementation), operationName);
                }

                if (!operationImplementationInfos.TryGetValue(new TypeNameList(parameterTypeNames), out OperationImplementationInfo implementation))
                {
                    throw new UnknownOperationOverloadException(typeof(TContractImplementation), operationName, parameterTypeNames);
                }

                return implementation;
            }
        }
    }
}