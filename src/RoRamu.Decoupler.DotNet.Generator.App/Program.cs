namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using RoRamu.Utils.CSharp;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (!args.Any())
            {
                return;
            }

            string assemblyFile = args[0];

            string @namespace = args.Length >= 2
                ? args[1]
                : "Generated.By.RoRamu.Decoupler";

            string outputDirectoryTransmitter = args.Length >= 3
                ? Path.GetFullPath(args[2])
                : Path.Combine(Directory.GetCurrentDirectory(), ".generated");

            string outputDirectoryReceiver = args.Length >= 4
                ? Path.GetFullPath(args[3])
                : Path.Combine(Directory.GetCurrentDirectory(), ".generated");

            string accessModifier = args.Length >= 5
                ? args[4]
                : CSharpAccessModifier.Public.ToString();

            Program.Run(assemblyFile, outputDirectoryTransmitter, outputDirectoryReceiver, accessModifier, @namespace);
        }

        public static void Run(string assemblyFile, string outputDirectoryTransmitter, string outputDirectoryReceiver, string accessModifier, string @namespace)
        {
            if (!File.Exists(assemblyFile))
            {
                throw new ArgumentException($"Assembly file '{assemblyFile}' does not exist.", nameof(assemblyFile));
            }

            if (!Directory.Exists(outputDirectoryTransmitter))
            {
                Directory.CreateDirectory(outputDirectoryTransmitter);
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentException("The provided namespace was empty.", nameof(@namespace));
            }

            if (!Enum.TryParse(accessModifier, out CSharpAccessModifier accessModifierEnum))
            {
                throw new ArgumentException($"Invalid access modifier '{accessModifier}'.  It must be one of the following: {string.Join(", ", Enum.GetNames(typeof(CSharpAccessModifier)))}", nameof(accessModifier));
            }

            Assembly assembly = Assembly.LoadFrom(assemblyFile);
            IEnumerable<Type> interfaces = Program.GetInterfaces(assembly);

            TransmitterGenerator transmitterGenerator = new();
            ReceiverGenerator receiverGenerator = new();
            foreach (Type @interface in interfaces)
            {
                // Build the contract definition from the interface
                ContractDefinition contract = InterfaceContractDefinitionBuilder.BuildContract(@interface);

                // Create the names for the transmitter and receiver classes
                string transmitterClassName = $"Transmitter_{@interface.GetCSharpName(identifierOnly: true, includeNamespace: false)}";
                string receiverClassName = $"Receiver_{@interface.GetCSharpName(identifierOnly: true, includeNamespace: false)}";

                // Generate the transmitter code
                string transmitterCode = transmitterGenerator.Run(contract, transmitterClassName, @namespace, accessModifierEnum);
                string transmitterOutputFilePath = Path.Combine(outputDirectoryTransmitter, $"{transmitterClassName}.cs");
                File.WriteAllText(transmitterOutputFilePath, transmitterCode);

                // Generate the receiver code
                string receiverCode = receiverGenerator.Run(contract, receiverClassName, @namespace, accessModifierEnum);
                string receiverOutputFilePath = Path.Combine(outputDirectoryReceiver, $"{receiverClassName}.cs");
                File.WriteAllText(receiverOutputFilePath, receiverCode);
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