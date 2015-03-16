using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Nancy;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// The body of a web API request
	/// </summary>
	public sealed class RequestBody : Notion, IRequestBody
	{
		private readonly NancyContext _context;
		private readonly Lazy<MediaType> _mediaType;
		private readonly Lazy<string> _text;
		private int _streamRead;

		public RequestBody(NancyContext context)
		{
			_context = context;

			_mediaType = new Lazy<MediaType>(() =>
			{
				return new MediaType(context.Request.Headers.ContentType);
			});

			_text = new Lazy<string>(() =>
			{
				using(var reader = new StreamReader(context.Request.Body))
				{
					return reader.ReadToEnd();
				}
			});
		}

		public readonly MediaType MediaType;

		public Media<string> ReadAsString()
		{
			return new Media<string>(_mediaType.Value, _text.Value);
		}

		public Media<Stream> ReadAsStream()
		{
			Expect(Interlocked.CompareExchange(ref _streamRead, value: 1, comparand: 0)).Is(1, "Cannot read stream twice");

			return new Media<Stream>(_mediaType.Value, _context.Request.Body);
		}
	}
}