namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains the behaviors for receiving an operation invocation.
    /// </summary>
    public interface IOperationInvocationReceiver
    {
        /// <summary>
        /// Gets a method which can execute the operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="parameterTypeNames">
        /// The type names of the parameters.  This must be in the same order as they were defined
        /// by the contract.
        /// </param>
        /// <param name="parameterTypes">The actual types of the parameters.  Useful for deserialization.</param>
        /// <returns>A method which can execute the operation.</returns>
        Delegates.ExecuteOperationFunc GetOperationImplementation(string operationName, IEnumerable<string> parameterTypeNames, out IEnumerable<Type> parameterTypes);
    }
}