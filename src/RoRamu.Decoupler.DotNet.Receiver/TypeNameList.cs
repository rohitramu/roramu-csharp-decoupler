namespace RoRamu.Decoupler.DotNet.Receiver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RoRamu.Utils.CSharp;

    public partial class Receiver<TContractImplementation>
    {
        /// <summary>
        /// Represents a list of type names.
        /// </summary>
        internal class TypeNameList : List<string>
        {
            /// <summary>
            /// Creates a <see cref="TypeNameList" />.
            /// </summary>
            /// <returns>A new <see cref="TypeNameList" />.</returns>
            public TypeNameList() : base()
            {

            }

            /// <summary>
            /// Creates a new <see cref="TypeNameList" />.
            /// </summary>
            /// <param name="typeNames">The type names to use as the initializer list.</param>
            /// <returns>A new <see cref="TypeNameList" />.</returns>
            public TypeNameList(IEnumerable<string> typeNames) : base(typeNames)
            {
                if (typeNames.Any(n => n == null))
                {
                    throw new ArgumentException("Found a null type name in the typeNames list.");
                }
            }

            /// <summary>
            /// Creates a new <see cref="TypeNameList" />.
            /// </summary>
            /// <param name="types">The types whose names to use as the initializer list.</param>
            /// <returns>A new <see cref="TypeNameList" />.</returns>
            public TypeNameList(IEnumerable<Type> types) : base(
                types?.Select(t => t.GetCSharpName()) // Should match ParameterValue.TypeCSharpName
                ?? throw new ArgumentNullException(nameof(types)))
            {

            }

            /// <inheritdoc />
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

            /// <inheritdoc />
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

            /// <inheritdoc />
            public override string ToString()
            {
                return string.Join(", ", this);
            }
        }
    }
}