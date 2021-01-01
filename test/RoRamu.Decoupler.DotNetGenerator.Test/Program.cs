namespace RoRamu.Decoupler.DotNetGenerator.Test
{
    using System;
    using Newtonsoft.Json;

    public class Program
    {
        public static void Main()
        {
            var builder = new InterfaceContractDefinitionBuilder(typeof(IMyContract));
            var contract = builder.BuildContract();

            string contractJson = JsonConvert.SerializeObject(contract, Formatting.Indented);
            Console.WriteLine(contractJson);
        }
    }
}