namespace RoRamu.Decoupler.DotNet.Generator.Transmitter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using RoRamu.Utils.CSharp;
    using System.Text;
    using System.Linq;
    using RoRamu.Utils;
    using System.Reflection;

    /// <summary>
    /// Generates a class which implements a given contract definition, using
    /// <see cref="Transmitter" /> as the base class.
    /// </summary>
    public class TransmitterGenerator : IContractDefinitionConsumer
    {
        /// <summary>
        /// The location of the generated file.
        /// </summary>
        public DirectoryInfo OutputDirectory { get; }

        /// <summary>
        /// The namespace of the class in the generated file.
        /// </summary>
        public string OutputNamespace { get; }

        /// <summary>
        /// The access level of the generated class.
        /// </summary>
        public CSharpAccessModifier ClassAccessModifier { get; }

        /// <summary>
        /// Creates a new <see cref="TransmitterGenerator" /> object.
        /// </summary>
        /// <param name="outputDirectory">The directory in which files should be generated.</param>
        /// <param name="outputNamespace">The namespace in which to put the generated class.</param>
        /// <param name="accessLevel">The access level of the generated class.</param>
        public TransmitterGenerator(string outputDirectory, string outputNamespace, CSharpAccessModifier accessLevel)
        {
            if (outputDirectory == null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            this.OutputNamespace = outputNamespace ?? throw new ArgumentNullException(nameof(outputNamespace));
            foreach (string namespacePart in outputNamespace.Split('.'))
            {
                if (!CSharpNamingUtils.IsValidIdentifier(namespacePart))
                {
                    throw new ArgumentException($"The provided namespace is invalid because the part '{namespacePart}' is not a valid C# identifier: {outputNamespace}", nameof(outputNamespace));
                }
            }

            this.ClassAccessModifier = accessLevel;

            // Validate the output directory and create it if it doesn't exist
            try
            {
                // Create the directory if it doesn't exist
                string directoryPath = Path.GetFullPath(outputDirectory);
                this.OutputDirectory = Directory.CreateDirectory(directoryPath);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Invalid directory path: {outputDirectory}", nameof(outputDirectory), e);
            }

            // Ensure that we can write files to the output directory
            string randomFileName = Path.GetRandomFileName();
            try
            {
                // Try to create a file in the directory to ensure permissions are set correctly
                string tempFilePath = Path.Combine(this.OutputDirectory.ToString(), randomFileName);
                File.Create(tempFilePath, 1, FileOptions.DeleteOnClose).Close();
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    $"Failed to create a temporary file '{randomFileName}' in the given output directory.  Does the folder have write permissions?",
                    nameof(outputDirectory),
                    e);
            }
        }

        /// <summary>
        /// Generates the transmitter class.
        /// </summary>
        /// <param name="contract">The contract for which to generate the transmitter class.</param>
        public void Run(ContractDefinition contract)
        {
            // Validate the contract
            this.ValidateContract(contract, out string implementationName);

            CSharpFile file = new CSharpFile(
                @namespace: this.OutputNamespace,
                usings: null,
                classes: this.GetClasses(contract, implementationName),
                fileHeader: this.GetFileHeader(contract)
            );

            this.OutputGeneratedCode(contract, file, implementationName);
        }

        /// <summary>
        /// Outputs the generated code (e.g. writes it to a file).
        /// </summary>
        /// <param name="contract">The contract which was used to generate the file.</param>
        /// <param name="file">The generated file.</param>
        /// <param name="implementationName">The name to give this implementation (e.g. filename, class name, etc.).</param>
        protected virtual void OutputGeneratedCode(ContractDefinition contract, CSharpFile file, string implementationName)
        {
            File.WriteAllText(
                path: Path.Combine(this.OutputDirectory.ToString(), $"{implementationName}.cs"),
                contents: file.ToString()
            );
        }

        /// <summary>
        /// Checks if the contract is valid, and throws if it is not.
        /// </summary>
        /// <param name="contract">The contract to check.</param>
        /// <param name="implementationName">The name to give this implementation (e.g. filename, class name, etc.).</param>
        protected virtual void ValidateContract(ContractDefinition contract, out string implementationName)
        {
            // Null check
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            // Make sure we have a valid name, as the generated class will have this name
            if (!CSharpNamingUtils.IsValidIdentifier(contract.Name))
            {
                throw new ArgumentException($"Contract name is not a valid C# identifier: {contract.Name}", nameof(contract));
            }

            implementationName = $"GeneratedTransmitter_{contract.Name}";
        }

        /// <summary>
        /// Gets the file header.
        /// </summary>
        /// <param name="contract">The contract which will be used when generating the file.</param>
        /// <returns>The file header.</returns>
        protected virtual CSharpComment GetFileHeader(ContractDefinition contract)
        {
            CSharpComment result = new CSharpComment($@"
DO NOT MODIFY - THIS IS AN AUTO-GENERATED FILE.

This file was generated by the <see cref=""{this.GetType().FullName}"" /> class.
".Trim()
            );

            return result;
        }

        private IEnumerable<CSharpClass> GetClasses(ContractDefinition contract, string implementationName)
        {
            Type baseClass = typeof(Transmitter);

            CSharpClass @class = new CSharpClass(
                implementationName,
                this.ClassAccessModifier,
                baseClass.GetCSharpName(),
                contract.Name.SingleObjectAsEnumerable(),
                null,
                null,
                this.GetConstructors(baseClass, implementationName), // TODO: Add constructor
                this.GetMethods(contract.Operations),
                new CSharpDocumentationComment(summary: null, rawNotes: contract.Description));

            yield return @class;
        }

        private IEnumerable<CSharpClassConstructor> GetConstructors(Type baseClass, string implementationName)
        {
            CSharpDocumentationComment docComment = new CSharpDocumentationComment(summary: null, rawNotes: "<inheritdoc />");

            foreach (ConstructorInfo constructorInfo in baseClass.GetConstructors())
            {
                IEnumerable<CSharpParameter> parameters = constructorInfo.GetParameters().Select(p => new CSharpParameter(p.Name, p.ParameterType.FullName));

                CSharpClassConstructor result = new CSharpClassConstructor(
                    implementationName,
                    CSharpAccessModifier.Public,
                    parameters,
                    parameters.Select(p => CSharpNamingUtils.SanitizeIdentifier(p.Name)),
                    null,
                    docComment
                );

                yield return result;
            }
        }

        private IEnumerable<CSharpMethod> GetMethods(IEnumerable<OperationDefinition> operations)
        {
            foreach (OperationDefinition operation in operations)
            {
                CSharpDocumentationComment docComment = new CSharpDocumentationComment(summary: null, rawNotes: operation.Description);
                bool isAsync = operation.IsAsync(out Type returnType);

                CSharpMethod result = new CSharpMethod(
                    operation.Name,
                    CSharpAccessModifier.Public,
                    isOverride: false,
                    isAsync: isAsync,
                    operation.ReturnType.GetCSharpName(),
                    this.GetMethodParameters(operation),
                    this.GetMethodBody(operation, isAsync, returnType),
                    docComment);

                yield return result;
            }
        }

        private IEnumerable<CSharpParameter> GetMethodParameters(OperationDefinition operation)
        {
            foreach (ParameterDefinition parameter in operation.Parameters)
            {
                CSharpParameter result = new CSharpParameter(
                    parameter.Name,
                    parameter.Type.GetCSharpName());

                yield return result;
            }
        }

        private string GetMethodBody(OperationDefinition operation, bool isAsync, Type returnType)
        {
            string parametersVarName = "_parameters";
            string operationVarName = "_operation";
            string resultVarName = "_result";

            StringBuilder sb = new StringBuilder();

            // Get the parameters to the operation invocation
            string parameterValueListTypeName = typeof(ParameterValueList).GetCSharpName();
            string parameterValueTypeName = typeof(ParameterValue).GetCSharpName();
            sb.AppendLine("// Create the parameters list if needed");
            sb.Append($"{parameterValueListTypeName} {parametersVarName} = ");
            if (!operation.Parameters.Any())
            {
                sb.AppendLine("null;");
            }
            else
            {
                // Create a new array of parameter values
                sb.AppendLine($"new {parameterValueListTypeName}(new {parameterValueTypeName}[] {{");

                // Add each parameter value
                foreach (ParameterDefinition parameter in operation.Parameters)
                {
                    string parameterName = CSharpNamingUtils.SanitizeIdentifier(parameter.Name);
                    sb.AppendLine($"{parameterValueTypeName}.{nameof(ParameterValue.Create)}(nameof({parameterName}), {parameter.Name}),".Indent());
                }

                // Close the array
                sb.AppendLine("});");
            }
            sb.AppendLine();

            // Create the operation invocation object
            string returnTypeNameString = returnType.GetCSharpName();
            string operationTypeNameString = returnType == typeof(void)
                ? $"{typeof(OperationInvocation).GetCSharpName(identifierOnly: true)}"
                : $"{typeof(OperationInvocation<>).GetCSharpName(identifierOnly: true)}<{returnTypeNameString}>";
            sb.AppendLine("// Create an object to represent the operation invocation");
            sb.AppendLine($"{operationTypeNameString} {operationVarName} = new {operationTypeNameString}(nameof({operation.Name}), {parametersVarName});");
            sb.AppendLine();

            // Call the appropriate transmit function, depending on whether or not it is 1) async or 2) needs to return a value
            sb.AppendLine("// Transmit the operation invocation so a receiver can forward it to an implementation which can execute it");
            if (returnType == typeof(void))
            { // Has no return value
                if (isAsync)
                {
                    sb.AppendLine($"await this.{nameof(Transmitter.RequestTransmitter)}.{nameof(IOperationInvocationTransmitter.TransmitMessageAsync)}({operationVarName});");
                }
                else
                {
                    sb.AppendLine($"this.{nameof(Transmitter.RequestTransmitter)}.{nameof(IOperationInvocationTransmitter.TransmitMessage)}({operationVarName});");
                }

                sb.AppendLine();
                sb.Append("return;");
            }
            else
            { // Has a return value
                if (isAsync)
                {
                    sb.AppendLine($"{returnTypeNameString} {resultVarName} = await this.{nameof(Transmitter.RequestTransmitter)}.{nameof(IOperationInvocationTransmitter.TransmitRequestAsync)}<{returnTypeNameString}>({operationVarName});");
                }
                else
                {
                    sb.AppendLine($"{returnTypeNameString} {resultVarName} = this.{nameof(Transmitter.RequestTransmitter)}.{nameof(IOperationInvocationTransmitter.TransmitRequest)}<{returnTypeNameString}>({operationVarName});");
                }

                sb.AppendLine();
                sb.Append($"return {resultVarName};");
            }

            string result = sb.ToString();
            return result;
        }
    }
}