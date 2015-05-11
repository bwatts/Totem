using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing the timeline and projecting one or more view types into the read model
	/// </summary>
	public abstract class Query : Flow
	{
		[Transient] protected IViewDb Views { get; private set; }

		protected override async Task MakeCall()
		{
			Views = Dependencies.Resolve<IViewDb>();

			try
			{
				await base.MakeCall();
			}
			finally
			{
				Views = null;
			}
		}

		protected void Write(View view)
		{
			Views.Write(view);
		}

		protected void Write(IEnumerable<View> views)
		{
			Views.Write(views);
		}

		protected void Write(params View[] views)
		{
			Write(views as IEnumerable<View>);
		}

		protected void Delete(View view)
		{
			Views.Delete(view);
		}

		protected void Delete(IEnumerable<View> views)
		{
			Views.Delete(views);
		}

		protected void Delete(params View[] views)
		{
			Delete(views as IEnumerable<View>);
		}

		protected void Delete(Type viewType, ViewKey key)
		{
			Views.Delete(viewType, key);
		}

		protected void Delete(Type viewType, IEnumerable<ViewKey> keys)
		{
			Views.Delete(viewType, keys);
		}

		protected void Delete(Type viewType, params ViewKey[] keys)
		{
			Delete(viewType, keys as IEnumerable<ViewKey>);
		}

		protected void Delete<TView>(ViewKey key) where TView : View
		{
			Views.Delete<TView>(key);
		}

		protected void Delete<TView>(IEnumerable<ViewKey> keys) where TView : View
		{
			Views.Delete<TView>(keys);
		}

		protected void Delete<TView>(params ViewKey[] keys) where TView : View
		{
			Delete<TView>(keys as IEnumerable<ViewKey>);
		}

		protected View Read(Type viewType, ViewKey key, bool strict = true)
		{
			return Views.Read(viewType, key, strict);
		}

		protected IEnumerable<View> Read(Type viewType, IEnumerable<ViewKey> keys, bool strict = true)
		{
			return Views.Read(viewType, keys, strict);
		}

		protected TView Read<TView>(ViewKey key, bool strict = true) where TView : View
		{
			return Views.Read<TView>(key, strict);
		}

		protected IEnumerable<TView> Read<TView>(IEnumerable<ViewKey> keys, bool strict = true) where TView : View
		{
			return Views.Read<TView>(keys, strict);
		}

		protected void Update<TView>(ViewKey key, Action<TView> update, bool strict = true) where TView : View
		{
			var view = Read<TView>(key, strict);

			if(view != null)
			{
				update(view);

				Write(view);
			}
		}

		protected void Update<TView>(IEnumerable<ViewKey> keys, Action<TView> update, bool strict = true) where TView : View
		{
			var views = Read<TView>(keys, strict).ToList();

			if(views.Any())
			{
				foreach(var view in views)
				{
					update(view);
				}

				Write(views);
			}
		}

		protected void CreateOrUpdate<TView>(ViewKey key, Func<TView> create, Action<TView> update) where TView : View
		{
			var view = Read<TView>(key, strict: false);

			if(view == null)
			{
				view = create();
			}
			else
			{
				update(view);
			}

			Write(view);
		}
	}

	/// <summary>
	/// A process observing the timeline and projecting the specified view type into the read model
	/// </summary>
	/// <typeparam name="TView">The type of view projected into the read model</typeparam>
	public abstract class Query<TView> : Flow where TView : View
	{
		[Transient] protected IViewDb Views { get; private set; }

		protected override async Task MakeCall()
		{
			Views = Dependencies.Resolve<IViewDb>();

			try
			{
				await base.MakeCall();
			}
			finally
			{
				Views = null;
			}
		}

		protected void Write(TView view)
		{
			Views.Write(view);
		}

		protected void Write(IEnumerable<TView> views)
		{
			Views.Write(views);
		}

		protected void Write(params TView[] views)
		{
			Write(views as IEnumerable<TView>);
		}

		protected void Delete(TView view)
		{
			Views.Delete(view);
		}

		protected void Delete(IEnumerable<TView> views)
		{
			Views.Delete(views);
		}

		protected void Delete(params TView[] views)
		{
			Delete(views as IEnumerable<TView>);
		}

		protected void Delete(ViewKey key)
		{
			Views.Delete(typeof(TView), key);
		}

		protected void Delete(IEnumerable<ViewKey> keys)
		{
			Views.Delete(typeof(TView), keys);
		}

		protected void Delete(params ViewKey[] keys)
		{
			Delete(keys as IEnumerable<ViewKey>);
		}

		protected TView Read(ViewKey key, bool strict = true)
		{
			return Views.Read<TView>(key, strict);
		}

		protected IEnumerable<TView> Read(IEnumerable<ViewKey> keys, bool strict = true)
		{
			return Views.Read<TView>(keys, strict);
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

		protected void CreateOrUpdate(ViewKey key, Func<TView> create, Action<TView> update)
		{
			var view = Read(key, strict: false);

			if(view == null)
			{
				view = create();
			}
			else
			{
				update(view);
			}

			Write(view);
		}
	}
}