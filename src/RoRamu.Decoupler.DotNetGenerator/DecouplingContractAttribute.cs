namespace RoRamu.Decoupler.DotNetGenerator
{
    using System;

    /// <summary>
    /// Identifies an interface as a contract which can be used to decouple components.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class DecouplingContractAttribute : Attribute
    {

    }
}
