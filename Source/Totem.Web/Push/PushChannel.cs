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
	/// A channel pushing events to remote collaborators
	/// </summary>
	/// <remarks>
	/// Adapted from http://www.asp.net/signalr/overview/getting-started/tutorial-server-broadcast-with-signalr#server
	/// </remarks>
	public sealed class PushChannel : IPushChannel
	{
		private readonly IHubContext _hubContext;

		public PushChannel(IHubContext hubContext)
		{
			_hubContext = hubContext;
		}

		public Task AddToGroup(Id connectionId, Id groupId)
		{
			return _hubContext.Groups.Add(connectionId.ToString(), groupId.ToString());
		}

		public Task RemoveFromGroup(Id connectionId, Id groupId)
		{
			return _hubContext.Groups.Remove(connectionId.ToString(), groupId.ToString());
		}

		public Task PushToAll(Event e)
		{
			return Push(e, to => to.All);
		}

		public Task PushToAllExcept(Event e, Many<Id> excludeConnectionIds)
		{
			return Push(e, to => to.AllExcept(ToStrings(excludeConnectionIds)));
		}

		public Task PushToClient(Event e, Id connectionId)
		{
			return Push(e, to => to.Client(connectionId.ToString()));
		}

		public Task PushToClients(Event e, Many<Id> connectionIds)
		{
			return Push(e, to => to.Clients(ToStrings(connectionIds)));
		}

		public Task PushToGroup(Event e, Id groupId, Many<Id> excludeConnectionIds)
		{
			return Push(e, to => to.Group(groupId.ToString(), ToStrings(excludeConnectionIds)));
		}

		public Task PushToGroups(Event e, Many<Id> groupIds, Many<Id> excludeConnectionIds)
		{
			return Push(e, to => to.Groups(ToStrings(groupIds), ToStrings(excludeConnectionIds)));
		}

		public Task PushToUser(Event e, Id userId)
		{
			return Push(e, to => to.User(userId.ToString()));
		}

		public Task PushToUsers(Event e, Many<Id> userIds)
		{
			return Push(e, to => to.Users(ToStrings(userIds)));
		}

		private async Task Push(Event e, Func<IHubConnectionContext<dynamic>, dynamic> selectClients)
		{
			var json = JsonFormat.Text.Serialize(e, typeof(Event));

			await selectClients(_hubContext.Clients).push(json);
		}

		private static string[] ToStrings(IEnumerable<Id> ids)
		{
			return ids.Select(id => id.ToString()).ToArray();
		}
	}
}