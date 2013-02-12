using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyTwitterStats.Json
{
	public class TwitterDateConverter : DateTimeConverterBase
	{
		private readonly int _addMinutes;

		public TwitterDateConverter(int timeZoneOffset = 0)
		{
			_addMinutes = timeZoneOffset * -1;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var s = (string)reader.Value;

			var dateTime = DateTime.ParseExact(s, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

			if (_addMinutes != 0)
			{
				dateTime = dateTime.AddMinutes(_addMinutes);
			}

			return dateTime;
		}
	}
}