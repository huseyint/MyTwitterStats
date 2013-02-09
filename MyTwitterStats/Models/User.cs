using System.Diagnostics;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Name}")]
	public class User
	{
		public string Name { get; set; }

		public string ScreenName { get; set; }

		public bool Protected { get; set; }

		public long Id { get; set; }

		public bool Verified { get; set; }

		public string ProfileImageUrlHttps { get; set; }
	}
}