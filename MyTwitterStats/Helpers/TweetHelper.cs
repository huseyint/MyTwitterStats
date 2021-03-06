﻿using System.Globalization;
using System.Web.Mvc;
using MyTwitterStats.Models;

namespace MyTwitterStats.Helpers
{
	public static class TweetHelper
	{
		public static MvcHtmlString Tweet(this HtmlHelper helper, Tweet tweet)
		{
			var p = new TagBuilder("p");
			p.SetInnerText(tweet.Text);

			var a = new TagBuilder("a");
			a.Attributes["href"] = string.Format("https://twitter.com/{0}/status/{1}", tweet.User.ScreenName, tweet.Id);
			a.SetInnerText(tweet.CreatedAt.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture));

			var blockquote = new TagBuilder("blockquote");
			blockquote.Attributes["class"] = "twitter-tweet";
			blockquote.InnerHtml = string.Format("{0}&mdash; {1} (@{2}) {3}", p, tweet.User.Name, tweet.User.ScreenName, a);

			return new MvcHtmlString(blockquote.ToString());
		}

		public static MvcHtmlString TwitterAccountLink(this HtmlHelper helper, string account)
		{
			var a = new TagBuilder("a");
			a.Attributes["href"] = string.Format("https://twitter.com/{0}", account);
			a.Attributes["target"] = "_blank";
			a.SetInnerText("@" + account);

			return new MvcHtmlString(a.ToString());
		}

		public static MvcHtmlString TwitterHashSearchLink(this HtmlHelper helper, string hash)
		{
			var a = new TagBuilder("a");
			a.Attributes["href"] = string.Format("https://twitter.com/search?q=%23{0}&src=hash", hash);
			a.Attributes["target"] = "_blank";
			a.SetInnerText("#" + hash);

			return new MvcHtmlString(a.ToString());
		}
	}
}