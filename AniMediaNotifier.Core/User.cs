using System;
using System.Collections.Generic;

namespace AniMediaNotifier.Core
{
	public class User
	{
		public Int32 Id { get; set; }
		public Sex Sex { get; set; }
		public String Name { get; set; }
		public Int64 PeerId { get; set; }

		public virtual ICollection<UserTitle> UserTitles { get; set; }
	}
}