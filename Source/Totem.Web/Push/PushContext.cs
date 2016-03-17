using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Totem.Runtime.Json;

namespace Totem.Web.Push
{
	/// <summary>
	/// The context of the push hub in the current runtime
	/// </summary>
	/// <remarks>
	/// Adapted from http://www.asp.net/signalr/overview/getting-started/tutorial-server-broadcast-with-signalr#server
	/// </remarks>
	public sealed class PushContext : IPushContext
	{
		private readonly IHubConnectionContext<dynamic> _clients;
		private readonly IGroupManager _groups;

		public PushContext()
		{
			var context = GlobalHost.ConnectionManager.GetHubContext<PushHub>();

			_clients = context.Clients;
			_groups = context.Groups;
		}

		public Task AddToGroup(Id connectionId, Id groupId)
		{
			return _groups.Add(connectionId.ToString(), groupId.ToString());
		}

		public Task RemoveFromGroup(Id connectionId, Id groupId)
		{
			return _groups.Remove(connectionId.ToString(), groupId.ToString());
		}

		public Task ToAll(Event e)
		{
			return CallPush(e, () => _clients.All);
		}

		public Task ToAllExcept(Event e, Many<Id> excludeConnectionIds)
		{
			return CallPush(e, () => _clients.AllExcept(ToStrings(excludeConnectionIds)));
		}

		public Task ToClient(Event e, Id connectionId)
		{
			return CallPush(e, () => _clients.Client(connectionId.ToString()));
		}

		public Task ToClients(Event e, Many<Id> connectionIds)
		{
			return CallPush(e, () => _clients.Clients(ToStrings(connectionIds)));
		}

		public Task ToGroup(Event e, Id groupId, Many<Id> excludeConnectionIds)
		{
			return CallPush(e, () => _clients.Group(groupId.ToString(), ToStrings(excludeConnectionIds)));
		}

		public Task ToGroups(Event e, Many<Id> groupIds, Many<Id> excludeConnectionIds)
		{
			return CallPush(e, () => _clients.Groups(ToStrings(groupIds), ToStrings(excludeConnectionIds)));
		}

		public Task ToUser(Event e, Id userId)
		{
			return CallPush(e, () => _clients.User(userId.ToString()));
		}

		public Task ToUsers(Event e, Many<Id> userIds)
		{
			return CallPush(e, () => _clients.Users(ToStrings(userIds)));
		}

		private static async Task CallPush(Event e, Func<dynamic> selectClients)
		{
			var json = JsonFormat.Text.Serialize(e, typeof(Event));

			await selectClients().push(json);
		}

		private static string[] ToStrings(IEnumerable<Id> ids)
		{
			return ids.Select(id => id.ToString()).ToArray();
		}
	}
}