using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Specs require less ceremony than traditional unit test definitions - they allow omission of
	/// the "public" modifier from classes and methods, as well as [Fact] annotations.
	/// 
	/// Adapted from http://patrick.lioi.net/2012/09/13/low-ceremony-xunit/
	/// </remarks>
	[XunitAdapter]
	public abstract class Specs
	{
    //
    // Expectations
    //

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected IExpect<T> Expect<T>(
			T value,
			[CallerMemberName] string caller = "",
			[CallerFilePath] string callerFile = "",
			[CallerLineNumber] int callerLine = 0)
		{
			return new SpecExpect<T>(value, caller, callerFile, callerLine);
		}

    private sealed class SpecExpect<T> : IExpect<T>
    {
      private readonly T _value;
      private readonly string _caller;
      private readonly string _callerFile;
      private readonly int _callerLine;

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      internal SpecExpect(T value, string caller, string callerFile, int callerLine)
      {
        _value = value;
        _caller = caller;
        _callerFile = callerFile;
        _callerLine = callerLine;
      }

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      public IExpect<T> IsTrue(Func<T, bool> check, Text message = null)
      {
        return Apply(() => Totem.Expect.That(_value).IsTrue(check), message);
      }

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      public IExpect<T> IsFalse(Func<T, bool> check, Text message = null)
      {
        return Apply(() => Totem.Expect.That(_value).IsFalse(check), message);
      }

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      private IExpect<T> Apply(Action expect, Text message)
      {
        try
        {
          expect();
        }
        catch(Exception error)
        {
          ThrowSpecException(message, error, _caller, _callerFile, _callerLine);
        }

        return this;
      }
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    private static void ThrowSpecException(
      Text message,
      Exception error,
      string caller,
      string callerFile,
      int callerLine)
    {
      message = Text
        .Of("Spec failed: ")
        .Write(caller)
        .Write(" (ln ")
        .Write(callerLine)
        .Write(" in ")
        .Write(callerFile)
        .Write(")")
        .WriteIf(message != null, Text.TwoLines + message);

      throw new SpecException(message, error);
    }

    //
    // Throws
    //

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected void ExpectThrows<TException>(
			Action action,
			Text message = null,
			[CallerMemberName] string caller = "",
			[CallerFilePath] string callerFile = "",
			[CallerLineNumber] int callerLine = 0)
			where TException : Exception
		{
      try
			{
				Totem.Expect.Throws<TException>(action, message);
			}
			catch(Exception error)
			{
        ThrowSpecException($"Unexpected exception type: {error.GetType()}", error, caller, callerFile, callerLine);
			}
		}

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected void ExpectThrows<TException>(
			Func<object> func,
			Text message = null,
			[CallerMemberName] string caller = "",
			[CallerFilePath] string callerFile = "",
			[CallerLineNumber] int callerLine = 0)
			where TException : Exception
		{
      try
      {
        Totem.Expect.Throws<TException>(func, message);
      }
      catch(Exception error)
      {
        ThrowSpecException($"Unexpected exception type: {error.GetType()}", error, caller, callerFile, callerLine);
      }
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