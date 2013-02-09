using System.Diagnostics;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Text}")]
	public class Hashtag
	{
		public string Text { get; set; }

		public int[] Indices { get; set; }
	}
}