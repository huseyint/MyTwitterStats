using System.Diagnostics;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Url}")]
	public class Media
	{
		public string Url { get; set; }

		public int[] Indices { get; set; }

		public string ExpandedUrl { get; set; }

		public string DisplayUrl { get; set; }
		
		public string MediaUrl { get; set; }
		
		public string MediaUrlHttps { get; set; }

		public long Id { get; set; }

		public MediaSize[] Sizes { get; set; }
	}
}