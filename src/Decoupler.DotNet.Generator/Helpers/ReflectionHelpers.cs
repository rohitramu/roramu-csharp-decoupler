namespace RoRamu.Decoupler.DotNet.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal static class ReflectionHelpers
    {
        public static IEnumerable<MethodInfo> GetMethods(IEnumerable<Type> interfaces)
        {
            foreach (Type @interface in interfaces)
            {
                // Get all members in this interface
                MemberInfo[] allMembers = @interface.GetMembers();

                // Make sure that all members are methods, and cast them to MethodInfo
                ICollection<MethodInfo> methods = new List<MethodInfo>(allMembers.Length);
                foreach (MemberInfo member in allMembers)
                {
                    if (member is MethodInfo method)
                    {
                        yield return method;
                    }
                    else
                    {
                        throw new InvalidMemberInInterfaceException(@interface, member, $"Only methods are allowed, but this member is a '{member.MemberType}'");
                    }
                }
            }
        }

        public static IEnumerable<Type> GetInheritedInterfaces(Type interfaceType, bool includeGivenInterface = true)
        {
            // Return the given interface if needed
            if (includeGivenInterface)
            {
                yield return interfaceType;
            }

            // Get the interfaces
            Type[] inheritedInterfaces = interfaceType.GetInterfaces();

            // Return the inherited interfaces
            foreach (Type @interface in inheritedInterfaces)
            {
                yield return @interface;
            }
        }
    }
}