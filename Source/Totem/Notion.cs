using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Runtime;
using Totem.Runtime.Map;

namespace Totem
{
	/// <summary>
	/// An object aware of Totem modeling techniques
	/// </summary>
	public abstract class Notion : IWritable, ITaggable
	{
		private Tags _tags;

		Tags ITaggable.Tags => Tags;

		protected Tags Tags => _tags ?? (_tags = new Tags());

		protected IClock Clock => Traits.Clock.Get(this);
		protected ILog Log => Traits.Log.Get(this);
		protected RuntimeMap Runtime => Traits.Runtime.Get(this);

		public sealed override string ToString() => ToText();
		public virtual Text ToText() => base.ToString();

		protected static Check<T> Check<T>(T target)
		{
			return Totem.Check.True(target);
		}

		protected static Check<T> CheckNot<T>(T target)
		{
			return Totem.Check.False(target);
		}

		protected static Expect<T> Expect<T>(T target)
		{
			return Totem.Expect.True(target);
		}

		protected static Expect<T> ExpectNot<T>(T target)
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

		public static class Traits
		{
			public static readonly Tag<IClock> Clock = Tag.Declare(() => Clock, new PlatformClock());
			public static readonly Tag<ILog> Log = Tag.Declare(() => Log, new UninitializedLog());
			public static readonly Tag<RuntimeMap> Runtime = Tag.Declare(() => Runtime);

			private sealed class PlatformClock : IClock
			{
				public DateTime Now
				{
					get { return DateTime.UtcNow; }
				}
			}

			//
			// Runtime
			//

			public static void InitializeRuntime(RuntimeMap runtime)
			{
				Expect(Runtime.ResolveDefault()).IsNull("The runtime trait is already initialized");

				Runtime.SetDefault(runtime);
			}

			//
			// Log
			//

			public static void InitializeLog(ILog effectiveLog)
			{
				var uninitializedLog = Log.ResolveDefault() as UninitializedLog;

				Expect(uninitializedLog).IsNotNull("The log is already initialized");

				Log.SetDefault(effectiveLog);

				uninitializedLog.ReplayMessages(effectiveLog);
			}

			private sealed class UninitializedLog : ILog
			{
				private readonly BlockingCollection<LogMessage> _messages = new BlockingCollection<LogMessage>();
				private ILog _effectiveLog;

				public LogLevel Level => LogLevel.Inherit;

				public void Write(LogMessage message)
				{
					if(_effectiveLog == null)
					{
						_messages.Add(message);
					}
					else
					{
						_effectiveLog.Write(message);
					}
				}

				internal void ReplayMessages(ILog effectiveLog)
				{
					_effectiveLog = effectiveLog;

					_messages.CompleteAdding();

					foreach(var message in _messages.GetConsumingEnumerable())
					{
						_effectiveLog.Write(message);
					}
				}
			}
		}
	}
}