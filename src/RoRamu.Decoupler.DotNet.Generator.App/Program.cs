namespace RoRamu.Decoupler.DotNet.Generator.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using CommandLine;
    using RoRamu.Utils.CSharp;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        public static void Run(Options options)
        {
            string assemblyFile = options.AssemblyFilePath;
            Component component = options.Component;
            string outputDirectory = options.OutputDirectory;
            string @namespace = options.Namespace;
            CSharpAccessModifier accessModifier = options.AccessModifier;

            if (!File.Exists(assemblyFile))
            {
                throw new ArgumentException($"Assembly file '{assemblyFile}' does not exist.", nameof(assemblyFile));
            }
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentException("The provided namespace was empty.", nameof(@namespace));
            }

            // Load the assembly which contains the interfaces to generate from
            Assembly assembly = Assembly.LoadFrom(assemblyFile);

            // Create the generator
            Generator generator = component switch
            {
                Component.Transmitter => new TransmitterGenerator(),
                Component.Receiver => new ReceiverGenerator(),
                _ => throw new ArgumentException("Unknown Decoupler component type.", nameof(Options.Component))
            };

            // Create the output directory if it doesn't exist
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Iterate over each interface, generating the output
            foreach (Type @interface in GetInterfaces(assembly))
            {
                // Build the contract definition from the interface
                ContractDefinition contract = InterfaceContractDefinitionBuilder.BuildContract(@interface);

                // Create the names for the transmitter and receiver classes
                string className = $"Generated{component}_{@interface.GetCSharpName(identifierOnly: true, includeNamespace: false)}";

                // Generate the transmitter code
                string code = generator.Run(contract, className, @namespace, accessModifier);
                string outputFilePath = Path.Combine(outputDirectory, $"{className}.cs");
                File.WriteAllText(outputFilePath, code);
            }
        }

        private static IEnumerable<Type> GetInterfaces(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                DecouplingContractAttribute attribute = type.GetCustomAttribute<DecouplingContractAttribute>(false);
                if (attribute != null)
                {
                    yield return type;
                }
            }
        }
    }
}