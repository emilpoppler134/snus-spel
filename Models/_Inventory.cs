using System;

namespace SnusSpel.Models
{
	public class _Inventory
	{
		public int Id { get; set; }
		public int CharacterId { get; set; }
		public int SnusId { get; set; }
		public int Amount { get; set; }
	}
}