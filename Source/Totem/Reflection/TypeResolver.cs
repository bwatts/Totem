using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Reflection
{
	/// <summary>
	/// Resolves types in the current app domain, correcting for partial assembly names
	/// </summary>
	public static class TypeResolver
	{
		// Adapted from Newtonsoft.Json.Utilities.ReflectionUtils.SplitFullyQualifiedTypeName
		//
		// https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ReflectionUtils.cs
		//
		// and Newtonsoft.Json.Serialization.DefaultSerializationBinder.BindToType
		//
		// https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Serialization/DefaultSerializationBinder.cs

		public static Type Resolve(string name, bool strict = true)
		{
			var type = _types.Resolve(name);

			Expect.False(strict && type == null, "Failed to resolve type: " + name);

			return type;
		}

		public static Type Resolve(string typeName, string assemblyName, bool strict = true)
		{
			var type = _types.Resolve(typeName, assemblyName);

			Expect.False(strict && type == null, Text.Of("Failed to resolve type {0} in assembly {1}", typeName, assemblyName));

			return type;
		}

		private static readonly TypeIndex _types = new TypeIndex();

		private sealed class TypeIndex
		{
			private readonly ConcurrentDictionary<string, Type> _typesByName = new ConcurrentDictionary<string, Type>();

			internal Type Resolve(string name)
			{
				var typeAndAssembly = SplitTypeAndAssembly(name);

				return _typesByName.GetOrAdd(name, _ => LoadType(typeAndAssembly.Item1, typeAndAssembly.Item2));
			}

			internal Type Resolve(string typeName, string assemblyName)
			{
				var name = typeName + ", " + assemblyName;

				return _typesByName.GetOrAdd(name, _ => LoadType(typeName, assemblyName));
			}

			private static Type LoadType(string typeName, string assemblyName)
			{
				if(assemblyName == null)
				{
					return Type.GetType(typeName);
				}

#pragma warning disable 618,612
				var assembly = Assembly.LoadWithPartialName(assemblyName);
#pragma warning restore 618,612

				if(assembly == null)
				{
					assembly = AppDomain.CurrentDomain
						.GetAssemblies()
						.Where(appDomainAssembly => appDomainAssembly.FullName == assemblyName || appDomainAssembly.GetName().Name == assemblyName)
						.FirstOrDefault();
				}

				return assembly == null ? null : assembly.GetType(typeName, throwOnError: false);
			}

			private static Tuple<string, string> SplitTypeAndAssembly(string type)
			{
				string name;
				string assembly;

				var assemblyDelimiterIndex = GetAssemblyDelimiterIndex(type);

				if(assemblyDelimiterIndex != null)
				{
					name = type.Substring(0, assemblyDelimiterIndex.Value).Trim();
					assembly = type.Substring(assemblyDelimiterIndex.Value + 1, type.Length - assemblyDelimiterIndex.Value - 1).Trim();
				}
				else
				{
					name = type;
					assembly = null;
				}

				return Tuple.Create(name, assembly);
			}

			private static int? GetAssemblyDelimiterIndex(string type)
			{
				// Get the first comma following all surrounded in brackets because of generic types
				//
				// e.g. System.Collections.Generic.Dictionary`2[[System.String, mscorlib,Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089

				var scope = 0;

				for(var i = 0; i < type.Length; i++)
				{
					var character = type[i];

					switch(character)
					{
						case '[':
							scope++;
							break;
						case ']':
							scope--;
							break;
						case ',':
							if(scope == 0)
							{
								return i;
							}

							break;
					}
				}

				return null;
			}
		}
	}
}