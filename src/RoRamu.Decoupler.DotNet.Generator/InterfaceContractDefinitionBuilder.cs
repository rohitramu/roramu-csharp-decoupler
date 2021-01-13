namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using RoRamu.Utils.CSharp;

    /// <summary>
    /// Produces a contract given a C# interface.
    /// </summary>
    public class InterfaceContractDefinitionBuilder : IContractDefinitionBuilder
    {
        /// <summary>
        /// The type of the interface for which a contract definition will be built.
        /// </summary>
        public Type InterfaceType { get; }

        private ContractDefinition ContractDefinition { get; set; }

        /// <summary>
        /// Creates a new <see cref="InterfaceContractDefinitionBuilder" />.
        /// </summary>
        /// <param name="interfaceType">The type of the interface for which a contract definition will be built.</param>
        public InterfaceContractDefinitionBuilder(Type interfaceType)
        {
            this.InterfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));

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
        public ContractDefinition Build()
        {
            // Return the cached result if we have one
            if (this.ContractDefinition != null)
            {
                return this.ContractDefinition;
            }

            // Get the XML documentation file for the assembly that contains the interface
            bool addDocs = this.InterfaceType.Assembly.TryGetXmlDocumentationFile(out XmlDocument xmlDocumentationFile);

            // Get all of the inherited interfaces
            IEnumerable<Type> interfaces = ReflectionHelpers.GetInheritedInterfaces(this.InterfaceType);

            // Get all of the methods in the interfaces
            // TODO: Deal with naming conflicts between methods from different interfaces
            IEnumerable<MethodInfo> methods = ReflectionHelpers.GetMethods(interfaces);

            // Add each method to the contract
            IList<OperationDefinition> operations = new List<OperationDefinition>();
            foreach (MethodInfo method in methods)
            {
                // Collect a list of the parameters
                IList<ParameterDefinition> parameters = new List<ParameterDefinition>();
                HashSet<string> seenParameterNames = new HashSet<string>();
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    // TODO: Validate that input and outputs are either value types or POCOs
                    // Validate that we don't have a duplicate parameter
                    if (seenParameterNames.Contains(parameter.Name))
                    {
                        throw new InvalidParameterInInterfaceMethodException(this.InterfaceType, method, parameter, $"Found duplicate parameter name: {parameter.Name}");
                    }

                    // Validate that the parameter is not an "out" or "ref" parameter
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
                    parameters.Add(new ParameterDefinition(parameter.Name, parameter.ParameterType));

                    // Mark the parameter name as "seen"
                    seenParameterNames.Add(parameter.Name);
                }

                // Create the operation
                OperationDefinition operation = new OperationDefinition(
                    name: method.Name,
                    returnType: method.ReturnType, // TODO: Validate that input and outputs are either value types or POCOs
                    description: addDocs
                        ? method.GetDocumentationComment(xmlDocumentationFile)
                        : null,
                    parameters: parameters
                );

                // Add the operation to the contract
                operations.Add(operation);
            }

            // Create the contract definition
            ContractDefinition contract = new ContractDefinition(
                name: this.InterfaceType.Name,
                fullName: this.InterfaceType.GetCSharpName(),
                description: addDocs
                    ? this.InterfaceType.GetDocumentationComment(xmlDocumentationFile)
                    : null,
                operations
            );

            // Cache the result so we don't re-calculate it every time
            this.ContractDefinition = contract;

            return contract;
        }

        private string GetInterfaceName()
        {
            string interfaceName = this.InterfaceType.Name;

            // If the name starts with an "I" (convention for interfaces in C#), then remove it.
            // NOTE: If the second character is not capitalized, the "I" is most likely part of the
            // first word rather than following the convention of starting interface names with an "I".
            if (interfaceName.Length >= 2 && interfaceName[0] == 'I' && char.IsUpper(interfaceName[1]))
            {
                // Take everything except the first character
                return interfaceName.Substring(1);
            }

            return interfaceName;
        }
    }
}