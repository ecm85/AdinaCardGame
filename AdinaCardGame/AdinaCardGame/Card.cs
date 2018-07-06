using System.Collections.Generic;

namespace Splendor
{
	public class Card
	{
		public string ResourceProduced { get; set; }
		public IDictionary<string, int> Costs { get; set; } = new Dictionary<string, int>();
		public int Points { get; set; }
		public int Tier { get; set; }
	}
}
