using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Text}")]
	public class Tweet
	{
		private static readonly Regex _sourceNameRegex = new Regex(">([^<]*)</a>");

		private static readonly Regex _sourceAddressRegex = new Regex(@"^<a href=""(https?://[a-z\d\.-]*/?)"" rel=""nofollow"">[^<]*</a>$", RegexOptions.IgnoreCase);

		private static readonly Uri _twitterWebAddress = new Uri("https://twitter.com/");

		public string Text { get; set; }

		public long Id { get; set; }

		public User User { get; set; }

		public string Source { get; set; }

		public DateTime CreatedAt { get; set; }

		public Entities Entities { get; set; }

		public RetweetedStatus RetweetedStatus { get; set; }

		public long InReplyToStatusId { get; set; }
		
		public long InReplyToUserId { get; set; }

		public Geo Geo { get; set; }

		// <a href="http://foursquare.com" rel="nofollow">foursquare</a>
		public string SourceName
		{
			get
			{
				if ("web".Equals(Source))
				{
					return Source;
				}

				var match = _sourceNameRegex.Match(Source);

				return match.Success ? match.Groups[1].Value : string.Empty;
			}
		}

		public Uri SourceAddress
		{
			get
			{
				if ("web".Equals(Source))
				{
					return _twitterWebAddress;
				}

				var match = _sourceAddressRegex.Match(Source);

				return match.Success ? new Uri(match.Groups[1].Value) : _twitterWebAddress;
			}
		}
	}
}