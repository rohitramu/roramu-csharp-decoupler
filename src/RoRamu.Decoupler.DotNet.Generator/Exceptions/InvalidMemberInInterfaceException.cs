namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using RoRamu.Utils.CSharp;

    /// <summary>
    /// Indicates that the provided type does not represent an interface.
    /// </summary>
    [Serializable]
    public class InvalidMemberInInterfaceException : DecouplerGeneratorException
    {
        /// <summary>
        /// The invalid member in the provided interface.
        /// </summary>
        public MemberInfo InvalidMember { get; }

        /// <inheritdoc/>
        public InvalidMemberInInterfaceException(Type @interface, MemberInfo invalidMember, string reason) : base(@interface, GetErrorMessage(@interface, invalidMember, reason))
        {
            this.InvalidMember = invalidMember;
        }

        /// <inheritdoc/>
        public InvalidMemberInInterfaceException(Type @interface, MemberInfo invalidMember, string reason, Exception innerException) : base(@interface, GetErrorMessage(@interface, invalidMember, reason), innerException)
        {
            this.InvalidMember = invalidMember;
        }

        private static string GetErrorMessage(Type @interface, MemberInfo invalidMember, string reason)
        {
            return $"The member '{invalidMember.Name}' of the interface '{@interface.GetCSharpName()}' is not allowed: {reason}.";
        }

        /// <inheritdoc/>
        protected InvalidMemberInInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                info.GetValue(nameof(this.InvalidMember), typeof(MemberInfo));
            }
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue(nameof(this.InvalidMember), this.InvalidMember);
            }
        }
    }
}
