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
    public static class InterfaceContractDefinitionBuilder
    {
        /// <summary>
        /// Builds the contract definition by treating each method in the interface as an operation.
        /// </summary>
        /// <returns>The contract definition.</returns>
        public static ContractDefinition BuildContract(Type interfaceType)
        {
            ValidateInterfaceType(interfaceType);

            // Get the XML documentation file for the assembly that contains the interface
            bool addDocs = interfaceType.Assembly.TryGetXmlDocumentationFile(out XmlDocument xmlDocumentationFile);

            // Get all of the inherited interfaces
            IEnumerable<Type> interfaces = ReflectionHelpers.GetInheritedInterfaces(interfaceType);

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
                        throw new InvalidParameterInInterfaceMethodException(interfaceType, method, parameter, $"Found duplicate parameter name: {parameter.Name}");
                    }

                    // Validate that the parameter is not an "out" or "ref" parameter
                    if (parameter.ParameterType.IsByRef)
                    {
                        // If "IsByRef" is false, the "IsOut" check may refer to the [out] attribute from System.Runtime.InteropServices, which does not indicate a C# "out" parameter.
                        // This is why we need to check "IsByRef" before checking "IsOut".
                        if (parameter.IsOut)
                        {
                            // Fail on "out" parameters
                            throw new InvalidParameterInInterfaceMethodException(interfaceType, method, parameter, $"'out' parameters are not allowed");
                        }

                        // Fail on "ref" parameters
                        throw new InvalidParameterInInterfaceMethodException(interfaceType, method, parameter, $"'ref' parameters are not allowed");
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
                name: interfaceType.Name,
                fullName: interfaceType.GetCSharpName(),
                description: addDocs
                    ? interfaceType.GetDocumentationComment(xmlDocumentationFile)
                    : null,
                operations
            );

            return contract;
        }

        private static string GetInterfaceName(Type interfaceType)
        {
            ValidateInterfaceType(interfaceType);

            string interfaceName = interfaceType.Name;

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

        private static void ValidateInterfaceType(Type interfaceType)
        {
            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            // Make sure the provided type is actually an interface
            if (!interfaceType.IsInterface)
            {
                throw new NotAnInterfaceException(interfaceType);
            }
        }
    }
}