using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Totem
{
	/// <summary>
	/// A suite of scenario-focused facts with low-ceremony definitions
	/// </summary>
	/// <remarks>
	/// Scenarios require less ceremony than traditional unit test definitions - they allow omission of the "public" modifier from classes and methods.
	/// 
	/// Adapted from http://patrick.lioi.net/2012/09/13/low-ceremony-xunit/
	/// </remarks>
	[Scenarios.FactsAttribute]
	public abstract class Scenarios
	{
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		private sealed class FactsAttribute : RunWithAttribute
		{
			internal FactsAttribute() : base(typeof(FactsCommand))
			{}
		}

		private sealed class FactsCommand : ITestClassCommand
		{
			private readonly TestClassCommand _defaultCommand = new TestClassCommand();

			public bool IsTestMethod(IMethodInfo testMethod)
			{
				return testMethod.MethodInfo.DeclaringType != typeof(object)
					&& testMethod.MethodInfo.DeclaringType != typeof(Scenarios)
					&& !testMethod.IsAbstract
					&& !testMethod.IsStatic
					&& testMethod.MethodInfo.ReturnType == typeof(void)
					&& testMethod.MethodInfo.GetParameters().Length == 0;
			}

			public IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
			{
				yield return new FactCommand(testMethod);
			}

			public IEnumerable<IMethodInfo> EnumerateTestMethods()
			{
				return TypeUnderTest.GetMethods().Where(IsTestMethod);
			}

			public object ObjectUnderTest
			{
				get { return _defaultCommand.ObjectUnderTest; }
			}

			public ITypeInfo TypeUnderTest
			{
				get { return _defaultCommand.TypeUnderTest; }
				set { _defaultCommand.TypeUnderTest = value; }
			}

			public int ChooseNextTest(ICollection<IMethodInfo> testsLeftToRun)
			{
				return _defaultCommand.ChooseNextTest(testsLeftToRun);
			}

			public Exception ClassStart()
			{
				try
				{
					foreach(var typeInterface in TypeUnderTest.Type.GetInterfaces())
					{
						if(typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == typeof(IUseFixture<>))
						{
							throw new NotSupportedException(GetType() + "does not support IUseFixture<>.");
						}
					}

					return null;
				}
				catch(Exception exception)
				{
					return exception;
				}
			}

			public Exception ClassFinish()
			{
				return null;
			}
		}
	}
}