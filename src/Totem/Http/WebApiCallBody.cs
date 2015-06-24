using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Totem.IO;

namespace Totem.Http
{
	/// <summary>
	/// The body of a request to a Totem web API
	/// </summary>
	public class WebApiCallBody : Notion
	{
		private readonly MediaType _mediaType;
		private readonly Func<string> _readAsString;
		private readonly Func<Stream> _readAsStream;

		public WebApiCallBody(MediaType mediaType, Func<string> readAsString, Func<Stream> readAsStream)
		{
			_mediaType = mediaType;
			_readAsString = readAsString;
			_readAsStream = readAsStream;

			CanRead = true;
		}

		public bool CanRead { get; private set; }

		public override Text ToText()
		{
			return _mediaType.ToText();
		}

		public Media<string> ReadAsString()
		{
			return Read(_readAsString);
		}

		public Media<Stream> ReadAsStream()
		{
			return Read(_readAsStream);
		}

		private Media<TContent> Read<TContent>(Func<TContent> read)
		{
			Expect(CanRead).IsTrue("Cannot read stream twice");

			CanRead = false;

			return new Media<TContent>(_mediaType, read());
		}

		public static WebApiCallBody From(MediaType mediaType, Func<string> readAsString, Func<Stream> readAsStream)
		{
			return new WebApiCallBody(mediaType, readAsString, readAsStream);
		}

		public static WebApiCallBody From(string mediaType, Func<string> readAsString, Func<Stream> readAsStream)
		{
			return From(new MediaType(mediaType), readAsString, readAsStream);
		}

		public static WebApiCallBody From(MediaType mediaType, Func<Stream> readAsStream)
		{
			Func<string> readAsString = () =>
			{
				using(var reader = new StreamReader(readAsStream()))
				{
					return reader.ReadToEnd();
				}
			};

			return From(mediaType, readAsString, readAsStream);
		}

		public static WebApiCallBody From(string mediaType, Func<Stream> readAsStream)
		{
			return From(new MediaType(mediaType), readAsStream);
		}
	}
}