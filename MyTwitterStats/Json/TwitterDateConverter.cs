using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyTwitterStats.Json
{
	public class TwitterDateConverter : DateTimeConverterBase
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var s = (string)reader.Value;

			var dateTime = DateTime.ParseExact(s, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);

			return dateTime;
		}
	}
}