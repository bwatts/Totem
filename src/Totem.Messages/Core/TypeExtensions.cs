using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Core
{
    public static class TypeExtensions
    {
        public static bool IsConcreteClass(this Type type)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsPublic && type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters;
        }

        public static bool IsGenericTypeDefinition(this Type type, Type definition)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(definition == null)
                throw new ArgumentNullException(nameof(definition));

            return type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == definition;
        }

        public static bool ImplementsGenericInterface(this Type type, Type interfaceDefinition)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(interfaceDefinition == null)
                throw new ArgumentNullException(nameof(interfaceDefinition));

            return type.GetInterfaces().Any(i => i.IsGenericTypeDefinition(interfaceDefinition));
        }

        public static IEnumerable<Type> GetInterfaceGenericArguments(this Type type, Type interfaceDefinition)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(interfaceDefinition == null)
                throw new ArgumentNullException(nameof(interfaceDefinition));

            return type.IsGenericTypeDefinition(interfaceDefinition) ? type.GetGenericArguments() : Enumerable.Empty<Type>();
        }

        public static IEnumerable<Type> GetImplementedInterfaceGenericArguments(this Type type, Type interfaceDefinition)
        {
            if(type == null)
                throw new ArgumentNullException(nameof(type));

            if(interfaceDefinition == null)
                throw new ArgumentNullException(nameof(interfaceDefinition));

            foreach(var typeInterface in type.GetInterfaces())
            {
                var foundArguments = false;

                foreach(var argument in typeInterface.GetInterfaceGenericArguments(interfaceDefinition))
                {
                    yield return argument;

                    foundArguments = true;
                }

                if(foundArguments)
                {
                    yield break;
                }
            }
        }
    }
}