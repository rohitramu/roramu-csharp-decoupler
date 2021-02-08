namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using RoRamu.Decoupler.DotNet.Receiver;
    using RoRamu.Utils;
    using RoRamu.Utils.CSharp;

    /// <summary>
    /// Generates a class which can forward calls to a given contract definition's implementation,
    /// using <see cref="Receiver" /> as the base class.
    /// </summary>
    public class ReceiverGenerator : Generator
    {
        private const string ImplementationParameterName = "impl";
        private const string OperationInvocationParameterName = "operationInvocation";
        private const string ParametersVariableName = "parameters";

        /// <inheritdoc />
        protected override CSharpFile GenerateCode(ContractDefinition contract, string implementationName, string @namespace, CSharpAccessModifier accessLevel)
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
            Type baseClass = typeof(Receiver<>);

            CSharpClass @class = new CSharpClass(
                name: implementationName,
                accessModifier: accessLevel,
                properties: this.GetProperties(contract),
                constructors: this.GetConstructors(contract, implementationName),
                methods: this.GetMethods(contract),
                baseType: $"{baseClass.GetCSharpName(identifierOnly: true)}<{contract.FullName}>",
                documentationComment: !string.IsNullOrWhiteSpace(contract.Description)
                    ? new CSharpDocumentationComment(summary: null, rawNotes: contract.Description)
                    : null);

            yield return @class;
        }

        private IEnumerable<CSharpProperty> GetProperties(ContractDefinition contract)
        {
            // Override property
            CSharpProperty operationsPropertyOverride = new CSharpProperty(
                "Operations",
                "OperationImplementationInfoCollection",
                CSharpAccessModifier.Protected,
                isOverride: true,
                hasSetter: false,
                defaultValue: "OperationsInternal",
                documentationComment: new CSharpDocumentationComment(null, "<inheritdoc />")
            );

            yield return operationsPropertyOverride;

            // Actual property implementation
            CSharpProperty operationsPropertyimplementation = new CSharpProperty(
                "OperationsInternal",
                "OperationImplementationInfoCollection",
                CSharpAccessModifier.Private,
                isStatic: true,
                defaultValue: this.GetOperationsPropertyValue(contract)
            );

            yield return operationsPropertyimplementation;
        }

        private string GetOperationsPropertyValue(ContractDefinition contract)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("new OperationImplementationInfoCollection");
            sb.AppendLine("{");

            foreach (OperationDefinition operation in contract.Operations)
            {
                sb.Append("{ ".Indent());
                sb.Append($"nameof({contract.FullName}.{operation.Name}), ");
                if (operation.Parameters.Any())
                {
                    var parameterTypes = string.Join(", ", operation.Parameters.Select(p => $"typeof({p.Type.GetCSharpName()})"));
                    sb.Append($"new {typeof(Type).GetCSharpName(identifierOnly: true)}[] {{ {parameterTypes} }}, ");
                }
                else
                {
                    sb.Append($"{typeof(Array).GetCSharpName()}.{nameof(Array.Empty)}<{typeof(Type).GetCSharpName()}>(), ");
                }
                sb.Append($"(impl) => (op) => {operation.Name}(impl, op)");
                sb.AppendLine("},");
            }

            sb.Append("}");

            string result = sb.ToString();

            return result;
        }

        private IEnumerable<CSharpClassConstructor> GetConstructors(ContractDefinition contract, string className)
        {
            CSharpDocumentationComment docComment = new CSharpDocumentationComment(summary: null, rawNotes: "<inheritdoc />");

            string contractImplementationParameterName = "contractImplementation";
            CSharpClassConstructor result = new CSharpClassConstructor(
                className,
                body: null,
                parameters: new CSharpParameter(contractImplementationParameterName, contract.FullName).SingleObjectAsEnumerable(),
                baseClassConstructorParameterValues: contractImplementationParameterName.SingleObjectAsEnumerable(),
                documentationComment: docComment
            );

            yield return result;
        }

        private IEnumerable<CSharpMethod> GetMethods(ContractDefinition contract)
        {
            IEnumerable<CSharpParameter> methodParameters = new CSharpParameter[]
            {
                new CSharpParameter(ImplementationParameterName, contract.FullName),
                new CSharpParameter(OperationInvocationParameterName, typeof(OperationInvocation).GetCSharpName()),
            };

            foreach (OperationDefinition operation in contract.Operations)
            {
                CSharpMethod result = new CSharpMethod(
                    name: operation.Name,
                    returnType: typeof(Task<object>).GetCSharpName(),
                    body: this.GetMethodBody(operation),
                    accessModifier: CSharpAccessModifier.Private,
                    parameters: methodParameters,
                    isStatic: true,
                    isAsync: true);

                yield return result;
            }
        }

        private string GetMethodBody(OperationDefinition operation)
        {
            StringBuilder sb = new StringBuilder();

            bool isAsync = operation.ReturnsTask(out Type returnType);
            bool returnsValue = returnType != typeof(void);
            int numParameters = operation.Parameters.Count();

            if (numParameters > 0)
            {
                sb.AppendLine($"var {ParametersVariableName} = {OperationInvocationParameterName}.{nameof(OperationInvocation.Parameters)};");
            }

            // Call implementation method
            if (returnsValue)
            {
                sb.Append("var result = ");
            }
            if (isAsync)
            {
                sb.Append($"await ");
            }
            sb.Append($"{ImplementationParameterName}.{operation.Name}(");

            // Add parameters
            if (numParameters == 1)
            {
                sb.Append($"{ParametersVariableName}[0].{nameof(ParameterValue.GetValue)}<{operation.Parameters.Single().Type.GetCSharpName()}>()");
            }
            else if (numParameters > 1)
            {
                bool first = true;
                for (int i = 0; i < operation.Parameters.Count; i++)
                {
                    ParameterDefinition parameter = operation.Parameters[i];

                    if (!first)
                    {
                        sb.Append(",");
                    }
                    first = false;
                    sb.AppendLine();

                    sb.Append($"{ParametersVariableName}[{i}].{nameof(ParameterValue.GetValue)}<{parameter.Type.GetCSharpName()}>()".Indent());
                }
                sb.AppendLine();
            }
            sb.AppendLine(");");

            // Return the result if any
            sb.AppendLine();
            if (returnsValue)
            {
                if (isAsync)
                {
                    sb.Append("return result;");
                }
                else
                {
                    sb.Append($"return await {typeof(Task).GetCSharpName()}.{nameof(Task.FromResult)}<object>(result);");
                }
            }
            else
            {
                sb.Append($"return await {typeof(Task).GetCSharpName()}.{nameof(Task.FromResult)}<{typeof(object).GetCSharpName()}>(null);");
            }

            string result = sb.ToString();
            return result;
        }
    }
}