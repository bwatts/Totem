using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Totem.IO;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Converts object to and from JSON in the text or binary formats
	/// </summary>
	public static class TotemJson
	{
		public static readonly TotemJsonFormat<Text> Text = new TextJsonFormat();
		public static readonly TotemJsonFormat<Binary> BinaryAscii = new BinaryJsonFormat(Encoding.ASCII);
		public static readonly TotemJsonFormat<Binary> BinaryUtf7 = new BinaryJsonFormat(Encoding.UTF7);
		public static readonly TotemJsonFormat<Binary> BinaryUtf8 = new BinaryJsonFormat(Encoding.UTF8);
		public static readonly TotemJsonFormat<Binary> BinaryUtf32 = new BinaryJsonFormat(Encoding.UTF32);
		public static readonly TotemJsonFormat<Binary> BinaryUnicode = new BinaryJsonFormat(Encoding.Unicode);

		// Adapted from Newtonsoft.Json.Utilities.ReflectionUtils.SplitFullyQualifiedTypeName
		//
		// https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ReflectionUtils.cs
		//
		// and Newtonsoft.Json.Serialization.DefaultSerializationBinder.BindToType
		//
		// https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Serialization/DefaultSerializationBinder.cs

		public static Type ResolveType(string name)
		{
			return _types.ResolveType(name);
		}

		public static Type ResolveType(string typeName, string assemblyName)
		{
			return _types.ResolveType(typeName, assemblyName);
		}

		private static readonly TypeIndex _types = new TypeIndex();

		private sealed class TypeIndex
		{
			private readonly ConcurrentDictionary<string, Type> _typesByName = new ConcurrentDictionary<string, Type>();

			internal Type ResolveType(string name)
			{
				var typeAndAssembly = SplitTypeAndAssembly(name);

				return _typesByName.GetOrAdd(name, _ => LoadType(typeAndAssembly.Item1, typeAndAssembly.Item2));
			}

			internal Type ResolveType(string typeName, string assemblyName)
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

				Expect.That(assembly).IsNotNull(Totem.Text.Of("Could not load assembly \"{0}\"", assemblyName));

				var assemblyType = assembly.GetType(typeName, throwOnError: false);

				Expect.That(assembly).IsNotNull(Totem.Text.Of("Could not find type \"{0}\" in assembly \"{1}\"", typeName, assembly.FullName));

				return assemblyType;
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