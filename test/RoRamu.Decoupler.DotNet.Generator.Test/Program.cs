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
    using RoRamu.Decoupler.DotNet.Generator.Transmitter;
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
            TestModel();
            // TestCSharpGeneration();

            Console.WriteLine();
            Console.WriteLine();

            // GeneratedTransmitter_IMyContract impl = new GeneratedTransmitter_IMyContract(new TestRequestTransmitter());
            // Console.WriteLine($"Result: {await impl.SayGoodbyeAsync("This is my goodbye message")}");
        }

        private class TestRequestTransmitter : IOperationInvocationTransmitter
        {
            public void TransmitMessage(OperationInvocation operationInvocation)
            {
                Console.WriteLine();
                Console.WriteLine($"== MESSAGE ==");
                Console.WriteLine(this.GetOperationDetails(operationInvocation));
            }

            public Task TransmitMessageAsync(OperationInvocation operationInvocation)
            {
                Console.WriteLine($"== MESSAGE ASYNC ==");
                Console.WriteLine(this.GetOperationDetails(operationInvocation));

                return Task.CompletedTask;
            }

            public T TransmitRequest<T>(OperationInvocation<T> operationInvocation)
            {
                Console.WriteLine($"== REQUEST ==");
                Console.WriteLine(this.GetOperationDetails(operationInvocation, typeof(T)));

                return default;
            }

            public Task<T> TransmitRequestAsync<T>(OperationInvocation<T> operationInvocation)
            {
                Console.WriteLine($"== REQUEST ASYNC ==");
                Console.WriteLine(this.GetOperationDetails(operationInvocation, typeof(T)));

                return Task.FromResult<T>(default);
            }

            private string GetOperationDetails(OperationInvocation operation, Type returnType = null)
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
                        sb.AppendLine($"{parameter.Type.GetCSharpName()} {parameter.Name} = {parameter.Value}".Indent());
                    }
                }

                string result = sb.ToString().TrimEnd();
                return result;
            }
        }

        private static void TestCSharpGeneration()
        {
            CSharpFile file = new(
                "RoRamu.Decoupler.DotNet.Generator.Test",
                null,
                new CSharpClass[]
                {
                    new CSharpClass(
                        "TestClass",
                        CSharpAccessModifier.Public,
                        null,
                        null,
                        null,
                        new CSharpProperty[]
                        {
                            new CSharpProperty(
                                "Name",
                                typeof(string).GetCSharpName(),
                                CSharpAccessModifier.Public,
                                null,
                                new CSharpDocumentationComment("Name of the person to greet.")
                            )
                        },
                        null,
                        new CSharpMethod[]
                        {
                            new CSharpMethod(
                                "Hello",
                                CSharpAccessModifier.Public,
                                false,
                                false,
                                typeof(string).GetCSharpName(),
                                new CSharpParameter[]
                                {
                                    new CSharpParameter("message", typeof(string).GetCSharpName(), "The message to show."),
                                },
@"
return $""Hello, {this.Name}!  {message}"";
".Trim(),
                                new CSharpDocumentationComment("Greets the person whose name is in the 'Name' property.")
                            )
                        },
                        new CSharpDocumentationComment("This is a generated test class!")
                    )
                },
                null
            );

            string fileContent = file.ToString();
            // File.WriteAllBytes("TestClass.cs", Encoding.UTF8.GetBytes(fileContent));
            Console.WriteLine(fileContent);
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

        private static void TestModel()
        {
            InterfaceContractDefinitionBuilder builder = new(typeof(IMyContract));
            ContractDefinition contract = builder.Build();

            TransmitterGenerator generator = new(
                ".run/",
                "RoRamu.Decoupler.DotNet.Generator.Test",
                CSharpAccessModifier.Public
            );
            generator.Run(contract);
            // string contractJson = JsonConvert.SerializeObject(contract, Formatting.Indented);
            // Console.WriteLine(contractJson);
        }
    }
}