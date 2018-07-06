using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
	public class Quest
	{
		public int Points { get; } = 3;
		public string Name { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }
		public IList<string> ToolRequirements { get; set; }

		public int ToolCountRequired => ToolRequirements.Count == 2 ? 4 : 3;
	}
}
