using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Runtime;
using Totem.Runtime.Configuration;
using Xunit;
using Xunit.Sdk;

namespace Totem
{
	/// <summary>
	/// A suite of scenario-focused facts with low-ceremony definitions
	/// </summary>
	/// <remarks>
	/// Specs require less ceremony than traditional unit test definitions - they allow omission of
	/// the "public" modifier from classes and methods, as well as [Fact] annotations.
	/// 
	/// Adapted from http://patrick.lioi.net/2012/09/13/low-ceremony-xunit/
	/// </remarks>
	[XunitAdapter]
	public abstract class Specs
	{
		static Specs()
		{
			var deployment = RuntimeSection.Read().ReadDeployment();

			var runtime = new RuntimeReader(deployment).Read();

			Notion.Traits.InitializeRuntime(runtime);
		}

    protected Expect<T> Expect<T>(T target)
		{
			return Totem.Expect.True(target);
		}

		protected Expect<T> ExpectNot<T>(T target)
		{
			return Totem.Expect.False(target);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void Expect(bool result)
		{
			Totem.Expect.True(result);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void Expect(bool result, Text issue)
		{
			Totem.Expect.True(result, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void ExpectNot(bool result)
		{
			Totem.Expect.False(result);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void ExpectNot(bool result, Text issue)
		{
			Totem.Expect.False(result, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		protected void ExpectThrows(Action action, Text issue = null)
		{
			Totem.Expect.Throws(action, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected void ExpectThrows<TException>(Action action, Text issue = null) where TException : Exception
		{
			Totem.Expect.Throws<TException>(action, issue);
		}

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected void ExpectThrows<TException>(Func<object> func, Text issue = null) where TException : Exception
		{
			Totem.Expect.Throws<TException>(func, issue);
		}

		//
		// xUnit smoothing
		//

		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
		private sealed class XunitAdapterAttribute : RunWithAttribute
		{
			internal XunitAdapterAttribute() : base(typeof(FactsCommand))
			{}
		}

		private sealed class FactsCommand : ITestClassCommand
		{
			private readonly TestClassCommand _defaultCommand = new TestClassCommand();

			public bool IsTestMethod(IMethodInfo testMethod)
			{
				return testMethod.MethodInfo.DeclaringType != typeof(object)
					&& testMethod.MethodInfo.DeclaringType != typeof(Specs)
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