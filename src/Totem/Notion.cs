using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

		Tags ITaggable.Tags { get { return Tags; } }

		protected Tags Tags { get { return _tags ?? (_tags = new Tags()); } }

		protected IClock Clock { get { return Traits.Clock.Get(this); } }
		protected ILog Log { get { return Traits.Log.Get(this); } }
		protected RuntimeMap Runtime { get { return Traits.Runtime.Get(this); } }

		public sealed override string ToString()
		{
			return ToText();
		}

		public virtual Text ToText()
		{
			return base.ToString();
		}

		protected static IExpect<T> Expect<T>(T value)
		{
			return Totem.Expect.That(value);
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
				Expect(Runtime.ResolveDefaultValue()).IsNull("The runtime trait is already initialized");

				Runtime.SetDefaultValue(runtime);
			}

			//
			// Log
			//

			public static void InitializeLog(ILog effectiveLog)
			{
				var uninitializedLog = Log.ResolveDefaultValue() as UninitializedLog;

				Expect(uninitializedLog).IsNotNull("The log is already initialized");

				Log.SetDefaultValue(effectiveLog);

				uninitializedLog.ReplayMessages(effectiveLog);
			}

			private sealed class UninitializedLog : ILog
			{
				private readonly BlockingCollection<LogMessage> _messages = new BlockingCollection<LogMessage>();

				public LogLevel Level { get { return LogLevel.Inherit; } }

				public void Write(LogMessage message)
				{
					_messages.Add(message);
				}

				internal void ReplayMessages(ILog effectiveLog)
				{
					_messages.CompleteAdding();

					foreach(var message in _messages.GetConsumingEnumerable())
					{
						effectiveLog.Write(message);
					}
				}
			}
		}
	}
}