namespace MyTwitterStats.Models
{
	public class Entities
	{
		public UserMention[] UserMentions { get; set; }

		public Hashtag[] Hashtags { get; set; }

		public TwitterUrl[] Urls { get; set; }

		public Media[] Media { get; set; }
	}
}