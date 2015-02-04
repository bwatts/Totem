using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Sdk;

namespace Totem
{
	/// <summary>
	/// A suite of scenario-focused facts with low-ceremony definitions
	/// </summary>
	/// <remarks>
	/// Scenarios require less ceremony than traditional unit test definitions - they allow omission of
	/// the "public" modifier from classes and methods, as well as [Fact] annotations.
	/// 
	/// Adapted from http://patrick.lioi.net/2012/09/13/low-ceremony-xunit/
	/// </remarks>
	[Scenarios.FactsAttribute]
	public abstract class Scenarios
	{
		//
		// Expectations
		//

		protected IExpect<T> Expect<T>(
			T value,
			[CallerMemberName] string caller = "",
			[CallerFilePath] string callerFile = "",
			[CallerLineNumber] int callerLine = 0)
		{
			return new ScenarioExpect<T>(value, caller, callerFile, callerLine);
		}

		protected void ExpectThrows<TException>(
			Action action,
			Text issue = null,
			[CallerMemberName] string caller = "",
			[CallerFilePath] string callerFile = "",
			[CallerLineNumber] int callerLine = 0)
			where TException : Exception
		{
			try
			{
				Totem.Expect.Throws<TException>(action, issue);
			}
			catch(ExpectException error)
			{
				throw new ScenarioException(error, caller, callerFile, callerLine);
			}
		}

		protected void ExpectThrows<TException>(
			Func<object> func,
			Text issue = null,
			[CallerMemberName] string caller = "",
			[CallerFilePath] string callerFile = "",
			[CallerLineNumber] int callerLine = 0)
			where TException : Exception
		{
			try
			{
				Totem.Expect.Throws<TException>(func, issue);
			}
			catch(ExpectException error)
			{
				throw new ScenarioException(error, caller, callerFile, callerLine);
			}
		}

		private sealed class ScenarioExpect<T> : IExpect<T>
		{
			private readonly IExpect<T> _that;
			private readonly string _caller;
			private readonly string _callerFile;
			private readonly int _callerLine;

			internal ScenarioExpect(T value, string caller, string callerFile, int callerLine)
			{
				_that = Totem.Expect.That(value);
				_caller = caller;
				_callerFile = callerFile;
				_callerLine = callerLine;
			}

			public IExpect<T> IsTrue(Func<T, bool> check, Text issue = null, Text expected = null, Func<T, Text> actual = null)
			{
				return Apply(() => _that.IsTrue(check, issue, expected, actual));
			}

			public IExpect<T> IsFalse(Func<T, bool> check, Text issue = null, Text expected = null, Func<T, Text> actual = null)
			{
				return Apply(() => _that.IsFalse(check, issue, expected, actual));
			}

			private IExpect<T> Apply(Action expect)
			{
				try
				{
					expect();

					return this;
				}
				catch(ExpectException error)
				{
					throw new ScenarioException(error, _caller, _callerFile, _callerLine);
				}
			}
		}

		//
		// xUnit smoothing
		//

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