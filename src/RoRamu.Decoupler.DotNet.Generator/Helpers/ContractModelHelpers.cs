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
        /// Determines whether or not an operation's return type is a <see cref="Task" />.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>True if the operation should be considered asynchronous, otherwise false.</returns>
        public static bool ReturnsTask(this OperationDefinition operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            Type type = operation.ReturnType;

            bool isAsync = type.IsGenericType
                ? type.GetGenericTypeDefinition() == typeof(Task<>)
                : type == typeof(Task);

            return isAsync;
        }

        /// <summary>
        /// Determines whether or not an operation's return type is a <see cref="Task" />.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="returnType">
        /// The return type of the operation.
        /// This unwraps the <see cref="Task{T}"/> type.
        /// </param>
        /// <returns>True if the operation should be considered asynchronous, otherwise false.</returns>
        public static bool ReturnsTask(this OperationDefinition operation, out Type returnType)
        {
            bool isAsync = operation.ReturnsTask();

            Type type = operation.ReturnType;

            // Unwrap the Task<T> type if necessary
            returnType = isAsync
                ? type.GenericTypeArguments?.SingleOrDefault() ?? typeof(void)
                : type;

            return isAsync;
        }
    }
}