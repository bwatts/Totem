using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing the timeline and maintaining a read model
	/// </summary>
	public abstract class Query : Flow
	{
		[Transient] protected new QueryCall Call { get { return (QueryCall) base.Call; } }
		[Transient] protected new QueryType Type { get { return (QueryType) base.Type; } }
	}

	/// <summary>
	/// A process observing the timeline and maintaining a view of the specified type
	/// </summary>
	/// <typeparam name="TView">The type of maintained view</typeparam>
	public abstract class Query<TView> : Query where TView : View
	{
		protected void Write(TView view)
		{
			Call.Views.Write(view);
		}

		protected void Write(IEnumerable<TView> views)
		{
			Call.Views.Write(views);
		}

		protected void Write(params TView[] views)
		{
			Write(views as IEnumerable<TView>);
		}

		protected void Delete(TView view)
		{
			Call.Views.Delete(view);
		}

		protected void Delete(IEnumerable<TView> views)
		{
			Call.Views.Delete(views);
		}

		protected void Delete(params TView[] views)
		{
			Delete(views as IEnumerable<TView>);
		}

		protected void Delete(ViewKey key)
		{
			Call.Views.Delete(typeof(TView), key);
		}

		protected void Delete(IEnumerable<ViewKey> keys)
		{
			Call.Views.Delete(typeof(TView), keys);
		}

		protected void Delete(params ViewKey[] keys)
		{
			Delete(keys as IEnumerable<ViewKey>);
		}

		protected TView Read(ViewKey key, bool strict = true)
		{
			return Call.Views.Read<TView>(key, strict);
		}

		protected IEnumerable<TView> Read(IEnumerable<ViewKey> keys, bool strict = true)
		{
			return Call.Views.Read<TView>(keys, strict);
		}

		protected void Update(ViewKey key, Action<TView> update, bool strict = true)
		{
			var view = Read(key, strict);

			if(view != null)
			{
				update(view);

				Write(view);
			}
		}

		protected void Update(IEnumerable<ViewKey> keys, Action<TView> update, bool strict = true)
		{
			var views = Read(keys, strict).ToList();

			if(views.Any())
			{
				foreach(var view in views)
				{
					update(view);
				}

				Write(views);
			}
		}

		protected void Upsert(ViewKey key, Func<TView> create, Action<TView> update)
		{
			var view = Read(key, strict: false);

			if(view == null)
			{
				view = create();
			}
			
			update(view);

			Write(view);
		}
	}
}