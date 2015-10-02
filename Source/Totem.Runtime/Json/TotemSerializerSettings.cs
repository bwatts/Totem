using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Settings used when serializing and deserializing objects in the Totem runtime
	/// </summary>
	public class TotemSerializerSettings : JsonSerializerSettings
	{
		public TotemSerializerSettings()
		{
			Formatting = Formatting.Indented;

			TypeNameHandling = TypeNameHandling.Auto;

			ContractResolver = new TotemContractResolver();

			Binder = new TotemSerializationBinder();

			Converters.AddRange(
				new StringEnumConverter(),
				new IsoDateTimeConverter
				{
					DateTimeStyles = DateTimeStyles.AssumeUniversal,
					DateTimeFormat = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern
				});

			DateTimeZoneHandling = DateTimeZoneHandling.Utc;
			DateFormatHandling = DateFormatHandling.IsoDateFormat;
		}

		public bool CamelCaseProperties
		{
			get
			{
				var knownResolver = ContractResolver as TotemContractResolver;

				return knownResolver != null && knownResolver.CamelCaseProperties;
			}
			set
			{
				var knownResolver = ContractResolver as TotemContractResolver;

				if(knownResolver != null)
				{
					knownResolver.CamelCaseProperties = value;
				}
			}
		}

		public JsonSerializer CreateSerializer()
		{
			return JsonSerializer.Create(this);
		}
	}
}