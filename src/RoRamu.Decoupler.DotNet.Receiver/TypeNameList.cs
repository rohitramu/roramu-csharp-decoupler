namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RoRamu.Utils.CSharp;

    public partial class Receiver<TContractImplementation>
    {
        protected class TypeNameList : List<string>
        {
            public TypeNameList() : base()
            {

            }

            public TypeNameList(IEnumerable<string> typeNames) : base(typeNames)
            {

            }

            public TypeNameList(IEnumerable<Type> types) : base(
                types?.Select(t => t.GetCSharpName()) // Should match ParameterValue.TypeCSharpName
                ?? throw new ArgumentNullException(nameof(types)))
            {

            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is TypeNameList other))
                {
                    return false;
                }

                if (this.Count != other.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] != other[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = 17;
                    foreach (string typeName in this)
                    {
                        result *= 23;
                        result += typeName.GetHashCode();
                    }

                    return result;
                }
            }

            public override string ToString()
            {
                return string.Join(", ", this);
            }
        }
    }
}