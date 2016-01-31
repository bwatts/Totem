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

		protected static Check<T> Check<T>(T target)
		{
			return Totem.Check.That(target);
		}

		protected static Expect<T> Expect<T>(T target)
		{
			return Totem.Expect.That(target);
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

				public LogLevel Level { get { return LogLevel.Inherit; } }

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