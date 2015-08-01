using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing the timeline and projecting the specified view type into the read model
	/// </summary>
	/// <typeparam name="TView">The type of view projected into the read model</typeparam>
	public abstract class Query<TView> : Flow where TView : View
	{
		[Transient] protected IViewDb Views { get; private set; }

		protected override async Task MakeWhenCall()
		{
			Views = Dependencies.Resolve<IViewDb>();

			try
			{
				await base.MakeWhenCall();
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