namespace RoRamu.Decoupler.DotNet.Generator.App
{
    using CommandLine;
    using RoRamu.Utils.CSharp;

    internal class Options
    {
        [Option(shortName: 'a', longName: "assembly", Required = true, HelpText = "The path to the assembly which contains the interface(s) to use as the decoupling contract.")]
        public string AssemblyFilePath { get; set; }

        [Option(shortName: 'c', longName: "component", Required = true, HelpText = "The Decoupler component to generate (e.g. transmitter or receiver).")]
        public Component Component { get; set; }

        [Option(shortName: 'o', longName: "outputDir", Default = ".generated", HelpText = "Where the class should be generated.")]
        public string OutputDirectory { get; set; }

        [Option(shortName: 'n', longName: "namespace", Default = "RoRamu.Decoupler.DotNet.Generated", HelpText = "The namespace of the generated class.")]
        public string Namespace { get; set; }

        [Option(shortName: 'm', longName: "accessModifier", Default = CSharpAccessModifier.Public, HelpText = "The access level of the generated class.")]
        public CSharpAccessModifier AccessModifier { get; set; }
    }
}