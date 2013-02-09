using System;
using System.Collections.Generic;

namespace MyTwitterStats.Models
{
	public class Stats
	{
		public Stats()
		{
			ThirdPartySourceCounts = new Dictionary<string, int>
			{
				{ "foursquare", 0 },
				{ "Instagram", 0 },
				{ "Tumblr", 0 },
				{ "RunKeeper", 0 },
			};
		}

		public int TotalTweetCount { get; set; }

		public Tweet FirstTweet { get; set; }

		public Tweet LastTweet { get; set; }

		public TimeSpan LifeSpan { get; set; }

		public double TweetsPerDay { get; set; }

		public string MostUsedClientName { get; set; }

		public int MostUsedClientCount { get; set; }

		public int RetweetCount { get; set; }

		public double RetweetsPerDay { get; set; }

		public string MostRetweetedAccountName { get; set; }

		public int MostRetweetedAccountCount { get; set; }

		public Tweet FastestRetweet { get; set; }

		public TimeSpan FastestRetweetSpan { get; set; }

		public int MentionCount { get; set; }

		public int ReplyCount { get; set; }

		public string MostMentionedAccountName { get; set; }

		public int MostMentionedAccountCount { get; set; }

		public string MostRepliedAccountName { get; set; }

		public int MostRepliedAccountCount { get; set; }

		public string MostUsedHashtagText { get; set; }

		public int MostUsedHashtagCount { get; set; }

		public string LongestHastag { get; set; }

		public string ShortestHastag { get; set; }

		public DayOfWeek MostTweetedDayOfWeekName { get; set; }

		public int MostTweetedDayOfWeekCount { get; set; }

		public DateTime MostTweetedDay { get; set; }

		public int MostTweetedDayCount { get; set; }

		public DayOfWeek LeastTweetedDayName { get; set; }

		public int LeastTweetedDayCount { get; set; }

		public int MostTweetedHour { get; set; }

		public int MostTweetedHourCount { get; set; }

		public int LeastTweetedHour { get; set; }

		public int LeastTweetedHourCount { get; set; }

		public TimeSpan LongestTimeNotTweeted { get; set; }

		public Tweet NotTweetedStartTweet { get; set; }

		public Tweet NotTweetedEndTweet { get; set; }

		public int TotalCharCount { get; set; }

		public int AverageCharCount { get; set; }

		public int TotalWordCount { get; set; }

		public int AverageWordCount { get; set; }

		public string MostUsedWord { get; set; }

		public int MostUsedWordCount { get; set; }

		public string MostDuplicatedTweet { get; set; }

		public int MostDuplicatedTweetCount { get; set; }

		public int LinkCount { get; set; }

		public string MostLinkedUrl { get; set; }

		public int MostLinkedUrlCount { get; set; }

		public string MostLinkedDomain { get; set; }

		public int MostLinkedDomainCount { get; set; }

		public Dictionary<string, int> ThirdPartySourceCounts { get; set; }
	}
}