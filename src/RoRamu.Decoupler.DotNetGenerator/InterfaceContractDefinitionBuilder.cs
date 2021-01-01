namespace RoRamu.Decoupler.DotNetGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Produces a contract given a C# interface.
    /// </summary>
    public class InterfaceContractDefinitionBuilder : IContractDefinitionBuilder
    {
        /// <summary>
        /// The type of the interface for which a contract definition will be built.
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// The namespace used in the generated output file.
        /// </summary>
        public string OutputNamespace { get; }

        private ContractDefinition ContractDefinition { get; set; }

        /// <summary>
        /// Creates a new <see cref="InterfaceContractDefinitionBuilder" />.
        /// </summary>
        /// <param name="interfaceType">The type of the interface for which a contract definition will be built.</param>
        /// <param name="outputNamespace">The namespace used in the generated output file.</param>
        public InterfaceContractDefinitionBuilder(Type interfaceType, string outputNamespace = null)
        {
            this.InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            this.OutputNamespace = outputNamespace ?? this.InterfaceType.Namespace;

            // Make sure the provided type is actually an interface
            if (!this.InterfaceType.IsInterface)
            {
                throw new NotAnInterfaceException(interfaceType);
            }
        }

        /// <summary>
        /// Builds the contract definition by treating each method in the interface as an operation.
        /// </summary>
        /// <returns>The contract definition.</returns>
        public ContractDefinition BuildContract()
        {
            // Return the cached result if we have one
            if (this.ContractDefinition != null)
            {
                return this.ContractDefinition;
            }

            // Get all of the inherited interfaces
            IEnumerable<Type> interfaces = ReflectionHelpers.GetInheritedInterfaces(this.InterfaceType);

            // Get all of the methods in the interfaces
            IEnumerable<MethodInfo> methods = ReflectionHelpers.GetMethods(interfaces);

            // Get the name of the contract
            string contractName = InterfaceContractDefinitionBuilder.GetContractNameFromInterface(this.InterfaceType);

            // Create the contract definition
            ContractDefinition result = new ContractDefinition(contractName);

            // Add each method to the contract
            foreach (MethodInfo method in methods)
            {
                // Create the operation
                OperationDefinition operation = new OperationDefinition(method.Name, method.ReturnType.FullName);

                // Add the parameters
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    // Validate the parameter.
                    if (parameter.ParameterType.IsByRef)
                    {
                        // If "IsByRef" is false, the "IsOut" check may refer to the [out] attribute from System.Runtime.InteropServices, which does not indicate a C# "out" parameter.
                        // This is why we need to check "IsByRef" before checking "IsOut".
                        if (parameter.IsOut)
                        {
                            // Fail on "out" parameters
                            throw new InvalidParameterInInterfaceMethodException(this.InterfaceType, method, parameter, $"'out' parameters are not allowed");
                        }

                        // Fail on "ref" parameters
                        throw new InvalidParameterInInterfaceMethodException(this.InterfaceType, method, parameter, $"'ref' parameters are not allowed");
                    }

                    // Add the parameter to the operation
                    operation.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.ParameterType.FullName));
                }

                // Add the operation to the contract
                result.Operations.Add(operation);
            }

            // Cache the result so we don't re-calculate it every time
            this.ContractDefinition = result;

            return result;
        }

        private static string GetContractNameFromInterface(Type interfaceType)
        {
            string interfaceName = interfaceType.Name;

            // If the name starts with an "I" (convention for interfaces in C#), then remove it.
            // NOTE: If the second character is not capitalized, the "I" is most likely part of the first word rather than following the convention.
            if (interfaceName.Length >= 2 && interfaceName[0] == 'I' && !char.IsUpper(interfaceName[1]))
            {
                // Take everything except the first character
                return interfaceName.Substring(1);
            }

            return interfaceName;
        }
    }
}