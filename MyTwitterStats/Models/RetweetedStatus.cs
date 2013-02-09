using System;
using System.Diagnostics;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Text}")]
	public class RetweetedStatus
	{
		public string Source { get; set; }

		public Entities Entities { get; set; }

		public Geo Geo { get; set; }

		public string Text { get; set; }

		public long Id { get; set; }

		public DateTime CreatedAt { get; set; }

		public User User { get; set; }
	}
}