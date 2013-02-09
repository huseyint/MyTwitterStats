﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MyTwitterStats.Json;
using MyTwitterStats.Models;
using Newtonsoft.Json;

namespace MyTwitterStats.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult UploadArchive(HttpPostedFileBase file)
		{
			var settings = new JsonSerializerSettings { ContractResolver = new UnderscoreMappingResolver() };
			settings.Converters.Add(new TwitterDateConverter());

			var serializer = JsonSerializer.Create(settings);

			var tweetsRaw = new List<Tweet>();

			using (var zipArchive = new ZipArchive(file.InputStream))
			{
				foreach (var entry in zipArchive.Entries.Where(e => Regex.Match(e.FullName, @"data/js/tweets/\d{4}_\d{2}\.js").Success))
				{
					using (var stream = entry.Open())
					using (var streamReader = new StreamReader(stream))
					{
						streamReader.ReadLine();

						var tweets = (Tweet[])serializer.Deserialize(streamReader, typeof(Tweet[]));

						tweetsRaw.AddRange(tweets);
					}
				}
			}

			var stats = GenerateStats(tweetsRaw);

			return View(stats);
		}

		private static Stats GenerateStats(List<Tweet> tweetsRaw)
		{
			var allTweets = tweetsRaw.OrderBy(t => t.CreatedAt).ToArray();

			var stats = new Stats();

			// A. GENERAL
			// A.1. Total tweets
			stats.TotalTweetCount = allTweets.Length;

			// A.2. Tweets per day
			stats.FirstTweet = allTweets.First();
			stats.LastTweet = allTweets.Last();
			stats.LifeSpan = stats.LastTweet.CreatedAt - stats.FirstTweet.CreatedAt;
			stats.TweetsPerDay = stats.TotalTweetCount/stats.LifeSpan.TotalDays;

			// A.3. Clients used
			var mostUsedClient = allTweets.GroupBy(t => t.SourceName).OrderByDescending(g => g.Count()).First();
			stats.MostUsedClientName = mostUsedClient.Key;
			stats.MostUsedClientCount = mostUsedClient.Count();

			// B. RETWEETS
			// B.1. Total retweets
			stats.RetweetCount = allTweets.Count(t => t.RetweetedStatus != null);

			// B.2. Retweets per day
			stats.RetweetsPerDay = stats.RetweetCount/stats.LifeSpan.TotalDays;

			// B.3. Most retweeted account
			var retweetedTweets = allTweets.Where(t => t.RetweetedStatus != null).ToArray();

			var mostRetweetedAccount = retweetedTweets
				.Select(t => t.RetweetedStatus)
				.GroupBy(rts => rts.User.ScreenName)
				.OrderByDescending(g => g.Count())
				.First();
			stats.MostRetweetedAccountName = mostRetweetedAccount.Key;
			stats.MostRetweetedAccountCount = mostRetweetedAccount.Count();

			// B.4. Fastest retweet
			var fastestRetweet = retweetedTweets
				.Select(t => new {Tweet = t, Delta = t.CreatedAt - t.RetweetedStatus.CreatedAt})
				.OrderBy(x => x.Delta).First();
			stats.FastestRetweet = fastestRetweet.Tweet;
			stats.FastestRetweetSpan = fastestRetweet.Delta;

			// C. MENTIONS & REPLIES
			// C.1. Mention count
			var mentions = allTweets.SelectMany(t => t.Entities.UserMentions).ToArray();
			stats.MentionCount = mentions.Length;

			// C.2. Reply count
			var replies = mentions.Where(um => um.IsReply).ToArray();
			stats.ReplyCount = replies.Length;

			// C.3. Most mentioned account
			var mostMentionedAccount = mentions.GroupBy(um => um.ScreenName).OrderByDescending(g => g.Count()).First();
			stats.MostMentionedAccountName = mostMentionedAccount.Key;
			stats.MostMentionedAccountCount = mostMentionedAccount.Count();

			// C.4. Most replied account
			var mostRepliedAccount = replies.GroupBy(um => um.ScreenName).OrderByDescending(g => g.Count()).First();
			stats.MostRepliedAccountName = mostRepliedAccount.Key;
			stats.MostRepliedAccountCount = mostRepliedAccount.Count();

			// D. HASHTAGS
			// D.1. Most used hashtag
			var hashtags = allTweets.SelectMany(t => t.Entities.Hashtags).ToArray();
			var mostUsedHashtag = hashtags.GroupBy(ht => ht.Text).OrderByDescending(g => g.Count()).First();
			stats.MostUsedHashtagText = mostUsedHashtag.Key;
			stats.MostUsedHashtagCount = mostUsedHashtag.Count();

			// D.2. Longest hashtag
			var hashtagsSortedByLength = hashtags.Select(ht => ht.Text).OrderBy(ht => ht.Length);
			stats.LongestHastag = hashtagsSortedByLength.Last();

			// D.3. Shortest hashtag
			stats.ShortestHastag = hashtagsSortedByLength.First();

			// E. DATE & TIME
			// E.1. Most tweeted day of week
			var tweetsGroupedByDay = allTweets.Select(t => t.CreatedAt.DayOfWeek).GroupBy(d => d).OrderBy(g => g.Count()).ToArray();
			var mostTweetedDayOfWeek = tweetsGroupedByDay.Last();
			stats.MostTweetedDayOfWeekName = mostTweetedDayOfWeek.Key;
			stats.MostTweetedDayOfWeekCount = mostTweetedDayOfWeek.Count();

			// E.2. Most tweeted day
			var mostTweetedDay = allTweets.GroupBy(t => t.CreatedAt.Date).OrderByDescending(g => g.Count()).First();
			stats.MostTweetedDay = mostTweetedDay.Key;
			stats.MostTweetedDayCount = mostTweetedDay.Count();

			// E.3. Least tweeted day
			var leastTweetedDay = tweetsGroupedByDay.First();
			stats.LeastTweetedDayName = leastTweetedDay.Key;
			stats.LeastTweetedDayCount = leastTweetedDay.Count();

			// E.4. Most tweeted hour
			var tweetsGroupedByHour = allTweets.Select(t => t.CreatedAt.Hour).GroupBy(d => d).OrderBy(g => g.Count());
			var mostTweetedHour = tweetsGroupedByHour.Last();
			stats.MostTweetedHour = mostTweetedHour.Key;
			stats.MostTweetedHourCount = mostTweetedHour.Count();

			// E.5. Least tweeted hour
			var leastTweetedHour = tweetsGroupedByHour.First();
			stats.LeastTweetedHour = leastTweetedHour.Key;
			stats.LeastTweetedHourCount = leastTweetedHour.Count();

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
			stats.NotTweetedStartTweet = startTweet;
			stats.NotTweetedEndTweet = endTweet;

			// F. TEXT ANALYSYS
			// F.1. Total char count
			stats.TotalCharCount = allTweets.Sum(t => t.Text.Length);

			// F.2. Average char count
			stats.AverageCharCount = stats.TotalCharCount/allTweets.Length;

			// F.3. Total word count
			var allWords =
				allTweets.SelectMany(t => t.Text.Split(new[] {' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
				         .ToArray();
			stats.TotalWordCount = allWords.Count();

			// F.4. Average word count
			stats.AverageWordCount = stats.TotalWordCount/allTweets.Length;

			// F.5. Most used word
			var mostUsedWord = allWords.Where(w => !"RT".Equals(w)).GroupBy(w => w).OrderByDescending(g => g.Count()).First();
			stats.MostUsedWord = mostUsedWord.Key;
			stats.MostUsedWordCount = mostUsedWord.Count();

			// F.6. Duplicate tweets
			var mostDuplicatedTweet =
				allTweets.GroupBy(t => t.Text).Where(g => g.Count() > 1).OrderByDescending(g => g.Count()).First();
			stats.MostDuplicatedTweet = mostDuplicatedTweet.Key;
			stats.MostDuplicatedTweetCount = mostDuplicatedTweet.Count();

			// G. LINKS
			// G.1. Link count
			var allUrls = allTweets.SelectMany(t => t.Entities.Urls).ToArray();
			stats.LinkCount = allUrls.Count();

			// G.2. Most linked URL
			var mostLinkedUrl = allUrls.Select(u => u.ExpandedUrl).GroupBy(u => u).OrderByDescending(g => g.Count()).First();
			stats.MostLinkedUrl = mostLinkedUrl.Key;
			stats.MostLinkedUrlCount = mostLinkedUrl.Count();

			// G.3. Most linked domain
			var mostLinkedDomain =
				allUrls.Select(u => new Uri(u.ExpandedUrl)).GroupBy(u => u.Host).OrderByDescending(g => g.Count()).First();
			stats.MostLinkedDomain = mostLinkedDomain.Key;
			stats.MostLinkedDomainCount = mostLinkedDomain.Count();

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