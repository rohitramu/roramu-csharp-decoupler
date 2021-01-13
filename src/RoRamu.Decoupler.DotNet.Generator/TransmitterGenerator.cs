namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RoRamu.Utils;
    using RoRamu.Utils.CSharp;
    using RoRamu.Decoupler.DotNet.Transmitter;
    using System.Reflection;

    /// <summary>
    /// Generates a class which implements a given contract definition, using
    /// <see cref="Transmitter" /> as the base class.
    /// </summary>
    public class TransmitterGenerator : Generator
    {
        /// <inheritdoc />
        protected override CSharpFile GenerateCode(
            ContractDefinition contract,
            string implementationName,
            string @namespace,
            CSharpAccessModifier accessLevel = CSharpAccessModifier.Public)
        {
            // Generate the code
            CSharpFile file = new CSharpFile(
                @namespace: @namespace,
                usings: null,
                classes: this.GetClasses(contract, implementationName, accessLevel),
                fileHeader: this.GetFileHeader(contract)
            );

            return file;
        }

        private IEnumerable<CSharpClass> GetClasses(ContractDefinition contract, string implementationName, CSharpAccessModifier accessLevel)
        {
            Type baseClass = typeof(Transmitter);

            CSharpClass @class = new CSharpClass(
                name: implementationName,
                accessModifier: accessLevel,
                constructors: this.GetConstructors(implementationName),
                methods: this.GetMethods(contract.Operations),
                baseType: baseClass.GetCSharpName(),
                interfaces: contract.FullName.SingleObjectAsEnumerable(),
                documentationComment: new CSharpDocumentationComment(summary: null, rawNotes: contract.Description));

            yield return @class;
        }

        private IEnumerable<CSharpClassConstructor> GetConstructors(string className)
        {
            CSharpDocumentationComment docComment = new CSharpDocumentationComment(summary: null, rawNotes: "<inheritdoc />");

            CSharpClassConstructor result = new CSharpClassConstructor(
                className,
                body: null,
                parameters: new CSharpParameter("requestTransmitter", typeof(IOperationInvocationTransmitter).GetCSharpName()).SingleObjectAsEnumerable(),
                baseClassConstructorParameterValues: "requestTransmitter".SingleObjectAsEnumerable(),
                documentationComment: docComment
            );

            yield return result;
        }

        private IEnumerable<CSharpMethod> GetMethods(IEnumerable<OperationDefinition> operations)
        {
            foreach (OperationDefinition operation in operations)
            {
                CSharpDocumentationComment docComment = new CSharpDocumentationComment(summary: null, rawNotes: operation.Description);
                bool isAsync = operation.ReturnsTask(out Type returnType);

                CSharpMethod result = new CSharpMethod(
                    name: operation.Name,
                    returnType: operation.ReturnType.GetCSharpName(),
                    body: this.GetMethodBody(operation, isAsync, returnType),
                    parameters: this.GetMethodParameters(operation),
                    isAsync: isAsync,
                    documentationComment: docComment);

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
            string parameterValueTypeName = typeof(ParameterValue).GetCSharpName();
            sb.AppendLine("// Create the parameters list if needed");
            sb.Append($"{parameterValueTypeName}[] {parametersVarName} = ");
            if (!operation.Parameters.Any())
            {
                sb.AppendLine("null;");
            }
            else
            {
                // Create a new array of parameter values
                sb.AppendLine($"new {parameterValueTypeName}[] {{");

                // Add each parameter value
                foreach (ParameterDefinition parameter in operation.Parameters)
                {
                    string parameterName = CSharpNamingUtils.SanitizeIdentifier(parameter.Name);
                    sb.AppendLine($"new {parameterValueTypeName}(nameof({parameterName}), {parameter.Name}, \"{parameter.Type.FullName}\"),".Indent());
                }

                // Close the array
                sb.AppendLine("};");
            }
            sb.AppendLine();

            // Create the operation invocation object
            bool returnsValue = returnType != typeof(void);
            string operationTypeNameString = typeof(OperationInvocation).GetCSharpName();
            sb.AppendLine("// Create an object to represent the operation invocation");
            sb.AppendLine($"{operationTypeNameString} {operationVarName} = new {operationTypeNameString}(nameof({operation.Name}), {parametersVarName}, {(returnsValue ? "true" : "false")});");
            sb.AppendLine();

            // Call the appropriate transmit function, depending on whether or not it is 1) async or 2) needs to return a value
            sb.AppendLine("// Transmit the operation invocation so a receiver can forward it to an implementation which can execute it");
            if (returnsValue)
            { // Has a return value
                string returnTypeNameString = returnType.GetCSharpName();
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
            else
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

            string result = sb.ToString();
            return result;
        }
    }
}