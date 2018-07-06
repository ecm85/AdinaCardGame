using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Splendor
{
	public class NewCard : Card
	{
		private static readonly IDictionary<string, string> materialsByResource = new Dictionary<string, string>
		{
			{ "Iron", "Iron" },
			{ "Dragonbone", "Dragonbone" },
			{ "Wood", "Wooden" },
			{ "Stone", "Stone" },
			{ "Magic", "Conjured" }
		};

		private static readonly IDictionary<Tuple<string, string>, string>  doubleMaterialsByResources = new Dictionary<Tuple<string, string>, string>
		{
			{Tuple.Create("Iron", "Wood"), "Ironwood"},
			{Tuple.Create("Iron", "Stone"), "Ironstone"},
			{Tuple.Create("Iron", "Dragonbone"), "Ironbone"},
			{Tuple.Create("Dragonbone", "Wood"), "Dragonwood"},
			{Tuple.Create("Dragonbone", "Stone"), "Dragonstone"},
			{Tuple.Create("Wood", "Stone"), "Woodstone"},
			{Tuple.Create("Magic", "Iron"), "Enchanted Iron"},
			{Tuple.Create("Magic", "Stone"), "Enchanted Stone"},
			{Tuple.Create("Magic", "Wood"), "Enchanted Wooden"},
			{Tuple.Create("Magic", "Dragonbone"), "Enchanted Dragonbone"}
		};

		private static readonly IDictionary<string, string> toolsByResourceProduced = new Dictionary<string, string>
		{
			{"Wood", "Axe" },
			{"Dragonbone", "Sword" },
			{"Stone", "Chisel" },
			{"Iron", "Pick" },
			{"Magic", "Staff" }
		};

		private static readonly IDictionary<int, string> qualitiesByPoints = new Dictionary<int, string>
		{
			{0, "Shoddy"},
			{1, "Standard" },
			{2, "Durable" },
			{3, "Strong" },
			{4, "Indestructible" },
			{5, "Godly" }
		};

		private static readonly IDictionary<string, Color> colorsByResourceProduced = new Dictionary<string, Color>
		{
			{"Wood", Color.SaddleBrown },
			{"Dragonbone", Color.White },
			{"Stone", Color.Gray },
			{"Iron", Color.Silver },
			{"Magic", Color.LightSkyBlue },
		};

		public string Name => $"{Quality} {Material} {Tool}";

		public string Tool => toolsByResourceProduced[ResourceProduced];

		public Color Color => colorsByResourceProduced[ResourceProduced];

		private string Material
		{
			get
			{
				switch (Costs.Count)
				{
					case 1:
						return materialsByResource[Costs.Keys.Single()];
					case 2:
						var materials = Costs.Keys.ToList();
						var firstDirection = Tuple.Create(materials[0], materials[1]);
						var secondDirection = Tuple.Create(materials[1], materials[0]);
						return doubleMaterialsByResources.ContainsKey(firstDirection) ?
							doubleMaterialsByResources[firstDirection] :
							doubleMaterialsByResources[secondDirection];
					case 3:
						var highestCount = Costs.Values.Max();
						return Costs.Values.Any(cost => cost <= highestCount - 2) ?
							materialsByResource[Costs.Keys.Single(key => Costs[key] == highestCount)] :
							"Composite";
					case 4:
						return "Composite";
				}
				return null;
			}
		}

		private string Quality
		{
			get
			{
				var highestCost = Costs.Values.Max();
				var lowestCost = Costs.Values.Min();
				var isBraced = Costs.Count == 3 && (highestCost - lowestCost > 1);
				var quality = qualitiesByPoints[Points];
				return isBraced ? quality + " Braced" : quality;
			}
		}
	}
}
