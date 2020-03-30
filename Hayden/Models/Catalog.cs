using Newtonsoft.Json;

namespace Hayden.Models
{
	public struct Catalog
	{
		[JsonProperty("page")]
		public uint PageNumber { get; set; }

		// In catalog each thread is a Post object with an extra last_replies that we don't care about
		[JsonProperty("threads")]
		public Post[] Threads { get; set; }
	}
}