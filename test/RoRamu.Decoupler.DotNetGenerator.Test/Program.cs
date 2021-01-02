namespace RoRamu.Decoupler.DotNet.Generator.Test
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using Newtonsoft.Json;
    using RoRamu.Utils.CSharp;

    public class Program
    {
        public static void Main()
        {
            // TestModel();
            TestCSharpGeneration();

            Console.WriteLine();
            Console.WriteLine();
        }

        private static void TestCSharpGeneration()
        {
            CSharpFile file = new CSharpFile(
                "Test.Generated",
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
                                typeof(string),
                                CSharpAccessModifier.Public,
                                null,
                                new CSharpDocumentationComment("Name of the person to greet.")
                            )
                        },
                        new CSharpMethod[]
                        {
                            new CSharpMethod(
                                "Hello",
                                CSharpAccessModifier.Public,
                                false,
                                false,
                                typeof(string).FullName,
                                new CSharpParameter[]
                                {
                                    new CSharpParameter("message", typeof(string), "The message to show."),
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
            InterfaceContractDefinitionBuilder builder = new InterfaceContractDefinitionBuilder(typeof(IMyContract));
            ContractDefinition contract = builder.Build();

            string contractJson = JsonConvert.SerializeObject(contract, Formatting.Indented);
            Console.WriteLine(contractJson);
        }
    }
}