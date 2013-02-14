using System.Globalization;

namespace MyTwitterStats.Models
{
	public class MonthTweetCount
	{
		public int Year { get; set; }

		public int Month { get; set; }

		public int Count { get; set; }

		public string MonthName
		{
			get { return Month > 0 ? CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames[Month - 1] : string.Empty; }
		}
	}
}