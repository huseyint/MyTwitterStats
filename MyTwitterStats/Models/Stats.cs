using System;
using System.Collections.Generic;
using System.Linq;

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

		public User OwnerAccount { get; set; }

		public int TotalTweetCount { get; set; }

		public Tweet FirstTweet { get; set; }

		public Tweet LastTweet { get; set; }

		public TimeSpan LifeSpan { get; set; }

		public double TweetsPerDay { get; set; }

		public string MostUsedClientName { get; set; }

		public Uri MostUsedClientAddress { get; set; }

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

		public MonthTweetCount[] MonthTweetCounts { get; set; }

		public MonthTweetCount MostTweetedMonth { get; set; }

		public DayOfWeek? MostTweetedDayOfWeekName { get; set; }

		public int MostTweetedDayOfWeekCount { get; set; }

		public DateTime? MostTweetedDay { get; set; }

		public int MostTweetedDayCount { get; set; }

		public DayOfWeek? LeastTweetedDayOfWeekName { get; set; }

		public int LeastTweetedDayCount { get; set; }

		public int MostTweetedHour { get; set; }

		public int MostTweetedHourCount { get; set; }

		public int LeastTweetedHour { get; set; }

		public int LeastTweetedHourCount { get; set; }

		public TimeSpan LongestTimeNotTweeted { get; set; }

		public DateTime NotTweetedStartDate { get; set; }

		public DateTime NotTweetedEndDate { get; set; }

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

		public static Stats GenerateStats(List<Tweet> tweetsRaw)
		{
			var allTweets = tweetsRaw.OrderBy(t => t.CreatedAt).ToArray();

			var stats = new Stats();

			if (allTweets.Length == 0)
			{
				return stats;
			}

			stats.OwnerAccount = allTweets.First().User;

			// A. GENERAL
			// A.1. Total tweets
			stats.TotalTweetCount = allTweets.Length;

			// A.2. Tweets per day
			stats.FirstTweet = allTweets.FirstOrDefault(t => t.RetweetedStatus == null);
			stats.LastTweet = allTweets.LastOrDefault(t => t.RetweetedStatus == null);

			if (stats.FirstTweet != null && stats.LastTweet != null)
			{
				stats.LifeSpan = stats.LastTweet.CreatedAt - stats.FirstTweet.CreatedAt;
				stats.TweetsPerDay = stats.TotalTweetCount / stats.LifeSpan.TotalDays;
			}

			// A.3. Clients used
			var mostUsedClient = allTweets.GroupBy(t => t.SourceName).OrderByDescending(g => g.Count()).FirstOrDefault();

			if (mostUsedClient != null && mostUsedClient.Any())
			{
				stats.MostUsedClientName = mostUsedClient.Key;
				stats.MostUsedClientAddress = mostUsedClient.First().SourceAddress;
				stats.MostUsedClientCount = mostUsedClient.Count();
			}

			// B. RETWEETS
			// B.1. Total retweets
			stats.RetweetCount = allTweets.Count(t => t.RetweetedStatus != null);

			// B.2. Retweets per day
			if (stats.LifeSpan.TotalDays > 0d)
			{
				stats.RetweetsPerDay = stats.RetweetCount / stats.LifeSpan.TotalDays;
			}

			// B.3. Most retweeted account
			var retweetedTweets = allTweets.Where(t => t.RetweetedStatus != null).ToArray();

			var mostRetweetedAccount = retweetedTweets
				.Select(t => t.RetweetedStatus)
				.GroupBy(rts => rts.User.ScreenName)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostRetweetedAccount != null)
			{
				stats.MostRetweetedAccountName = mostRetweetedAccount.Key;
				stats.MostRetweetedAccountCount = mostRetweetedAccount.Count();
			}

			// B.4. Fastest retweet
			var fastestRetweet = retweetedTweets
				.Select(t => new { Tweet = t, Delta = t.CreatedAt - t.RetweetedStatus.CreatedAt })
				.OrderBy(x => x.Delta)
				.FirstOrDefault();

			if (fastestRetweet != null)
			{
				stats.FastestRetweet = fastestRetweet.Tweet;
				stats.FastestRetweetSpan = fastestRetweet.Delta;
			}

			// C. MENTIONS & REPLIES
			// C.1. Mention count
			var mentions = allTweets.SelectMany(t => t.Entities.UserMentions).ToArray();
			stats.MentionCount = mentions.Length;

			// C.2. Reply count
			var replies = mentions.Where(um => um.IsReply).ToArray();
			stats.ReplyCount = replies.Length;

			// C.3. Most mentioned account
			var mostMentionedAccount = mentions
				.GroupBy(um => um.ScreenName)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostMentionedAccount != null)
			{
				stats.MostMentionedAccountName = mostMentionedAccount.Key;
				stats.MostMentionedAccountCount = mostMentionedAccount.Count();
			}

			// C.4. Most replied account
			var mostRepliedAccount = replies
				.GroupBy(um => um.ScreenName)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostRepliedAccount != null)
			{
				stats.MostRepliedAccountName = mostRepliedAccount.Key;
				stats.MostRepliedAccountCount = mostRepliedAccount.Count();
			}

			// D. HASHTAGS
			// D.1. Most used hashtag
			var hashtags = allTweets.SelectMany(t => t.Entities.Hashtags).ToArray();
			var mostUsedHashtag = hashtags
				.GroupBy(ht => ht.Text)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostUsedHashtag != null)
			{
				stats.MostUsedHashtagText = mostUsedHashtag.Key;
				stats.MostUsedHashtagCount = mostUsedHashtag.Count();
			}

			// D.2. Longest hashtag
			var hashtagsSortedByLength = hashtags
				.Select(ht => ht.Text)
				.OrderBy(ht => ht.Length);
			stats.LongestHastag = hashtagsSortedByLength.LastOrDefault();

			// D.3. Shortest hashtag
			stats.ShortestHastag = hashtagsSortedByLength.FirstOrDefault();

			// E. DATE & TIME
			// E.1. Most tweeted day of week
			var tweetsGroupedByDay = allTweets
				.Select(t => t.CreatedAt.DayOfWeek)
				.GroupBy(d => d)
				.OrderBy(g => g.Count())
				.ToArray();
			var mostTweetedDayOfWeek = tweetsGroupedByDay.LastOrDefault();

			if (mostTweetedDayOfWeek != null)
			{
				stats.MostTweetedDayOfWeekName = mostTweetedDayOfWeek.Key;
				stats.MostTweetedDayOfWeekCount = mostTweetedDayOfWeek.Count();
			}

			// E.2. Most tweeted day
			var mostTweetedDay = allTweets
				.GroupBy(t => t.CreatedAt.Date)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostTweetedDay != null)
			{
				stats.MostTweetedDay = mostTweetedDay.Key;
				stats.MostTweetedDayCount = mostTweetedDay.Count();
			}

			// E.3. Least tweeted day
			var leastTweetedDay = tweetsGroupedByDay.FirstOrDefault();

			if (leastTweetedDay != null)
			{
				stats.LeastTweetedDayOfWeekName = leastTweetedDay.Key;
				stats.LeastTweetedDayCount = leastTweetedDay.Count();
			}

			// E.5. Most tweeted month
			stats.MonthTweetCounts = allTweets
				.Select(t => Tuple.Create(t.CreatedAt.Year, t.CreatedAt.Month))
				.GroupBy(t => t)
				.Select(g => new MonthTweetCount { Year = g.Key.Item1, Month = g.Key.Item2, Count = g.Count() })
				.ToArray();

			stats.MostTweetedMonth = stats.MonthTweetCounts.OrderByDescending(mtc => mtc.Count).FirstOrDefault();

			// E.5. Most tweeted hour
			var tweetsGroupedByHour = allTweets
				.Select(t => t.CreatedAt.Hour)
				.GroupBy(d => d)
				.OrderBy(g => g.Count());
			var mostTweetedHour = tweetsGroupedByHour.LastOrDefault();

			if (mostTweetedHour != null)
			{
				stats.MostTweetedHour = mostTweetedHour.Key;
				stats.MostTweetedHourCount = mostTweetedHour.Count();
			}

			// E.5. Least tweeted hour
			var leastTweetedHour = tweetsGroupedByHour.FirstOrDefault();

			if (leastTweetedHour != null)
			{
				stats.LeastTweetedHour = leastTweetedHour.Key;
				stats.LeastTweetedHourCount = leastTweetedHour.Count();
			}

			// E.6. Longest time not tweeted
			var previousTweet = stats.FirstTweet;
			var startTweet = previousTweet;
			var endTweet = previousTweet;
			var longestTime = new TimeSpan();

			for (var i = 1; i < allTweets.Length; i++)
			{
				var currentTweet = allTweets[i];
				var delta = currentTweet.CreatedAt - previousTweet.CreatedAt;

				if (delta > longestTime)
				{
					startTweet = previousTweet;
					endTweet = currentTweet;
					longestTime = delta;
				}

				previousTweet = currentTweet;
			}

			stats.LongestTimeNotTweeted = longestTime;
			stats.NotTweetedStartDate = startTweet.CreatedAt;
			stats.NotTweetedEndDate = endTweet.CreatedAt;

			// F. TEXT ANALYSYS
			// F.1. Total char count
			stats.TotalCharCount = allTweets.Sum(t => t.Text.Length);

			// F.2. Average char count
			if (allTweets.Length > 0)
			{
				stats.AverageCharCount = stats.TotalCharCount / allTweets.Length;
			}

			// F.3. Total word count
			var allWords = allTweets
				.SelectMany(t => t.Text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
				.ToArray();
			stats.TotalWordCount = allWords.Count();

			// F.4. Average word count
			if (allTweets.Length > 0)
			{
				stats.AverageWordCount = stats.TotalWordCount / allTweets.Length;
			}

			// F.5. Most used word
			var mostUsedWord = allWords
				.Where(w => !"RT".Equals(w))
				.GroupBy(w => w).OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostUsedWord != null)
			{
				stats.MostUsedWord = mostUsedWord.Key;
				stats.MostUsedWordCount = mostUsedWord.Count();
			}

			// F.6. Duplicate tweets
			var mostDuplicatedTweet = allTweets
				.GroupBy(t => t.Text)
				.Where(g => g.Count() > 1)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostDuplicatedTweet != null)
			{
				stats.MostDuplicatedTweet = mostDuplicatedTweet.Key;
				stats.MostDuplicatedTweetCount = mostDuplicatedTweet.Count();
			}

			// G. LINKS
			// G.1. Link count
			var allUrls = allTweets.SelectMany(t => t.Entities.Urls).ToArray();
			stats.LinkCount = allUrls.Count();

			// G.2. Most linked URL
			var mostLinkedUrl = allUrls
				.Select(u => u.ExpandedUrl)
				.GroupBy(u => u)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostLinkedUrl != null)
			{
				stats.MostLinkedUrl = mostLinkedUrl.Key;
				stats.MostLinkedUrlCount = mostLinkedUrl.Count();
			}

			// G.3. Most linked domain
			var mostLinkedDomain = allUrls
				.Select(u => new Uri(u.ExpandedUrl))
				.GroupBy(u => u.Host)
				.OrderByDescending(g => g.Count())
				.FirstOrDefault();

			if (mostLinkedDomain != null)
			{
				stats.MostLinkedDomain = mostLinkedDomain.Key;
				stats.MostLinkedDomainCount = mostLinkedDomain.Count();
			}

			// H. 3RD PARTY
			foreach (var tweet in allTweets)
			{
				int count;

				if (stats.ThirdPartySourceCounts.TryGetValue(tweet.SourceName, out count))
				{
					stats.ThirdPartySourceCounts[tweet.SourceName] = count + 1;
				}
			}

			return stats;
		}
	}
}