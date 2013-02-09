using System.Diagnostics;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Name}")]
	public class UserMention
	{
		public string Name { get; set; }

		public string ScreenName { get; set; }

		public int[] Indices { get; set; }

		public long Id { get; set; }

		public bool IsReply
		{
			get { return Indices.Length > 0 && Indices[0] == 0; }
		}
	}
}