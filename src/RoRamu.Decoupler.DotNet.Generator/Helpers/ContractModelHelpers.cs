namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension helper methods for objects which represent parts of a contract.
    /// </summary>
    public static class ContractModelHelpers
    {
        /// <summary>
        /// Determines whether an operation should be considered asynchronous.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="returnType">
        /// The return type of the operation.
        /// This unwraps the <see cref="Task{T}"/> type.
        /// </param>
        /// <returns>True if the operation should be considered asynchronous, otherwise false.</returns>
        public static bool IsAsync(this OperationDefinition operation, out Type returnType)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            Type type = operation.ReturnType;

            bool isAsync = type.IsGenericType
                ? type.GetGenericTypeDefinition() == typeof(Task<>)
                : type == typeof(Task);

            // Unwrap the Task<T> type if necessary
            returnType = isAsync
                ? type.GenericTypeArguments?.SingleOrDefault() ?? typeof(void)
                : type;

            return isAsync;
        }
    }
}