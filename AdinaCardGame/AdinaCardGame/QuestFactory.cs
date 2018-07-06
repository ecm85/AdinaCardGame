using System.Collections.Generic;

namespace Splendor
{
	public static class QuestFactory
	{
		//sword+staff fight dragons
		//axe sword fight treeguard warriors
		//axe chisel clear ground for city
		//chisel and pick mining expedition
		//staff and pick summon iron golem labourers
		//axe chisel pick gathering expedition
		//staff pick chisel magical gathering or stone golem
		//staff sword axe fight treeguard spell casters
		//pick staff sword click create armed golems
		//chisel axe sword raze enemy castle

		public static IEnumerable<Quest> CreateQuests()
		{
			return new[]
			{
				new Quest
				{
					Name = "Defeat Dragon Army",
					Description = "A dragon army approaches! Arm your villagers with swords and staves to defeat them.",
					ToolRequirements = new[] {"Sword", "Staff"},
					Image = "Dragons"
				},
				new Quest
				{
					Name = "Defeat Treeguard Fighters",
					Description = "Rumors report treeguard fighters terrorizing local foresters. Send your villagers in to hack them down.",
					ToolRequirements = new[] {"Sword", "Axe"},
					Image = "Ent terrorizing"
				},
				new Quest
				{
					Name = "Clear Ground for City",
					Description = "We need a new city but there are pesky trees and mountains in the way. Clear the way!",
					ToolRequirements = new[] {"Axe", "Chisel"},
					Image = "Trees and Rocks"
				},
				new Quest
				{
					Name = "Mining Expedition",
					Description = "The king needs resources to build a fantastic Wonder. Find all the iron and stone you can!",
					ToolRequirements = new[] {"Chisel", "Pick"},
					Image = "Cave"
				},
				new Quest
				{
					Name = "Summon Iron Golem Servants",
					Description = "We've lost far too many villagers in the mines. Summon Iron Golems to work in the mines in their stead.",
					ToolRequirements = new [] {"Staff", "Pick"},
					Image = "Iron Golem"
				},
				new Quest
				{
					Name = "Prepare for Winter",
					Description = "Winter is coming! Gather resources that will be needed while the passes are snowed in.",
					ToolRequirements = new [] {"Axe", "Chisel", "Pick"},
					Image = "snowy outside 1"
				},
				new Quest
				{
					Name = "Magical Gathering",
					Description = "The king's mages are tired from summoning crystals. Go out and find rich magic veins so they can recover.",
					ToolRequirements = new [] {"Staff", "Chisel", "Pick"},
					Image = "Glowing Ore"
				},
				new Quest
				{
					Name = "Defeat Treeguard Sorcerer",
					Description = "The old treeguard sorcerer has held sway over this region long enough. End his evil reign!",
					ToolRequirements = new [] {"Sword", "Axe", "Staff"},
					Image = "Ent"
				},
				new Quest
				{
					Name = "Summon Iron Golem Warriors",
					Description = "The king is having trouble recruiting enough soldiers. Summon Iron Golem Warriors to fill his army.",
					ToolRequirements = new [] {"Sword", "Staff", "Pick"},
					Image = "Armed Iron Golem"
				},
				new Quest
				{
					Name = "Raze Enemy Castle",
					Description = "The neighboring king has farted in our general direction. Show him the error of his ways!",
					ToolRequirements = new [] {"Pick", "Sword", "Axe"},
					Image = "monty python castle"
				}
			};
		}
	}
}
