namespace RoRamu.Decoupler.DotNet.Generator.Transmitter
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

            string outputDirectory = args.Length >= 2
                ? Path.GetFullPath(args[1])
                : Directory.GetCurrentDirectory();

            string accessModifier = args.Length >= 3
                ? args[2]
                : CSharpAccessModifier.Public.ToString();

            string @namespace = args.Length >= 4
                ? args[3]
                : "Generated.By.RoRamu.Decoupler";

            Program.Run(assemblyFile, outputDirectory, accessModifier, @namespace);
        }

        private static void Run(string assemblyFile, string outputDirectory, string accessModifier, string @namespace)
        {
            if (!File.Exists(assemblyFile))
            {
                throw new ArgumentException($"Assembly file '{assemblyFile}' does not exist.", nameof(assemblyFile));
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
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

            TransmitterGenerator generator = new(outputDirectory, @namespace, accessModifierEnum);
            foreach (Type @interface in interfaces)
            {
                ContractDefinition contract = new InterfaceContractDefinitionBuilder(@interface).Build();
                generator.Run(contract);
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