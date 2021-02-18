namespace RoRamu.Decoupler.DotNet.Generator.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using RoRamu.Decoupler.DotNet;
    using RoRamu.Decoupler.DotNet.Receiver;
    using RoRamu.Decoupler.DotNet.Transmitter;
    using RoRamu.Utils;
    using RoRamu.Utils.CSharp;

    /// <summary>
    ///
    /// </summary>
    public class Program
    {
        /// <summary>
        ///
        /// </summary>
        public static async Task Main()
        {
            await GenerateCode();

            // await RunGeneratedCode();

            Console.WriteLine();
            Console.WriteLine();
        }

        private static void CompileAndRun(string fileContent, string codeToRun)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(fileContent);
            string fileName = "mylib.dll";

            // Detect the file location for the library that defines the object type
            var systemRefLocation = typeof(object).GetTypeInfo().Assembly.Location;

            // Create a reference to the library
            var systemReference = MetadataReference.CreateFromFile(systemRefLocation);

            // A single, immutable invocation to the compiler
            // to produce a library
            var compilation = CSharpCompilation
                .Create(fileName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(systemReference)
                .AddSyntaxTrees(tree);

            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            EmitResult compilationResult = compilation.Emit(path);
            if (compilationResult.Success)
            {
                // Load the assembly
                Assembly asm =
                  AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                // Invoke the RoslynCore.Helper.CalculateCircleArea method passing an argument
                double radius = 10;
                object result =
                  asm.GetType("RoslynCore.Helper").GetMethod("CalculateCircleArea").
                  Invoke(null, new object[] { radius });
                Console.WriteLine($"Circle area with radius = {radius} is {result}");
            }
            else
            {
                foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
                {
                    string issue = $@"
ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()},
Location: { codeIssue.Location.GetLineSpan()},
Severity: { codeIssue.Severity}
".Trim();
                    Console.WriteLine(issue);
                }
            }
        }

        private static async Task GenerateCode()
        {
            ContractDefinition contract = InterfaceContractDefinitionBuilder.BuildContract(typeof(IMyContract));

            string transmitterImplementationName = $"GeneratedTransmitter_{contract.Name}";
            TransmitterGenerator transmitterGenerator = new();
            string generatedTransmitterFile = transmitterGenerator.Run(
                contract,
                transmitterImplementationName,
                "RoRamu.Decoupler.DotNet.Generator.Test"
            );


            string receiverImplementationName = $"GeneratedReceiver_{contract.Name}";
            ReceiverGenerator receiverGenerator = new();
            string generatedReceiverFile = receiverGenerator.Run(
                contract,
                receiverImplementationName,
                "RoRamu.Decoupler.DotNet.Generator.Test"
            );


            string directory = Path.GetFullPath("generated");
            Directory.CreateDirectory(directory);

            Console.WriteLine(generatedTransmitterFile);
            await File.WriteAllTextAsync(Path.Combine(directory, $"{transmitterImplementationName}.cs"), generatedTransmitterFile);

            Console.WriteLine(generatedReceiverFile);
            await File.WriteAllTextAsync(Path.Combine(directory, $"{receiverImplementationName}.cs"), generatedReceiverFile);


        }

        private static void RunGeneratedCode()
        {
            // IMyContract impl = new GeneratedTransmitter_IMyContract(new LocalRequestTransmitter<IMyContract>(new GeneratedReceiver_IMyContract(new MyImplementation())));
            // var response = await impl.SayGoodbyeAsync("Harjaap");
            // Console.WriteLine($"!!Result: {response}");

            // var receiver = new GeneratedReceiver_IMyContract(new MyImplementation());
            // var operationImpl = receiver.GetOperationImplementation(nameof(IMyContract.SayGoodbye), typeof(string).GetCSharpName().SingleObjectAsEnumerable(), out _);
            // string returnObj = (string)await operationImpl(new OperationInvocation(nameof(IMyContract.SayGoodbye), new ParameterValue("message", "my friend!").SingleObjectAsEnumerable(), true));
            // Console.WriteLine(returnObj);
        }

        private class MyImplementation : IMyContract
        {
            public string SayGoodbye(string message)
            {
                return $"Goodbye, {message}";
            }

            public Task<string> SayGoodbyeAsync(string message)
            {
                return Task.FromResult(this.SayGoodbye(message));
            }

            public void SayHello(long val, IMyContract test)
            {

            }

            public void SayNothing()
            {

            }

            public Task SayNothingAsync()
            {
                return Task.CompletedTask;
            }
        }

        private class TestRequestTransmitter : IOperationInvocationTransmitter
        {
            public void TransmitMessage(OperationInvocation operationInvocation)
            {
                Console.WriteLine();
                Console.WriteLine($"== MESSAGE ==");
                Console.WriteLine(GetOperationDetails(operationInvocation));
            }

            public Task TransmitMessageAsync(OperationInvocation operationInvocation)
            {
                Console.WriteLine($"== MESSAGE ASYNC ==");
                Console.WriteLine(GetOperationDetails(operationInvocation));

                return Task.CompletedTask;
            }

            public T TransmitRequest<T>(OperationInvocation operationInvocation)
            {
                Console.WriteLine($"== REQUEST ==");
                Console.WriteLine(GetOperationDetails(operationInvocation, typeof(T)));

                return default;
            }

            public Task<T> TransmitRequestAsync<T>(OperationInvocation operationInvocation)
            {
                Console.WriteLine($"== REQUEST ASYNC ==");
                Console.WriteLine(GetOperationDetails(operationInvocation, typeof(T)));

                return Task.FromResult<T>(default);
            }

            private static string GetOperationDetails(OperationInvocation operation, Type returnType = null)
            {
                StringBuilder sb = new();
                sb.AppendLine($"Operation: {operation.Name}");
                if (returnType != null)
                {
                    sb.AppendLine($"Return type: {returnType.GetCSharpName()}");
                }
                if (operation.Parameters.Any())
                {
                    sb.AppendLine("Parameters:");
                    foreach (ParameterValue parameter in operation.Parameters)
                    {
                        sb.AppendLine($"{parameter.CSharpTypeName} {parameter.Name} = {parameter.Value}".Indent());
                    }
                }

                string result = sb.ToString().TrimEnd();
                return result;
            }
        }
    }
}