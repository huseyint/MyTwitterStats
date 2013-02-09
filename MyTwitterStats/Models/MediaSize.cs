using System.Diagnostics;
using Newtonsoft.Json;

namespace MyTwitterStats.Models
{
	[DebuggerDisplay("{Resize} - {Width}x{Height}")]
	public class MediaSize
	{
		public string Resize { get; set; }

		[JsonProperty("H")]
		public int Height { get; set; }

		[JsonProperty("W")]
		public int Width { get; set; }
	}
}