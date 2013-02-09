using System.Diagnostics;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Url}")]
	public class TwitterUrl
	{
		public string Url { get; set; }

		public int[] Indices { get; set; }

		public string ExpandedUrl { get; set; }
		
		public string DisplayUrl { get; set; }
	}
}