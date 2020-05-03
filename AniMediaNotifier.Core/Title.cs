using System;
using System.Collections.Generic;

namespace AniMediaNotifier.Core
{
	public class Title
	{
		public Int32 Id { get; set; }
		public String Name { get; set; }
		public TitleStatus Status { get; set; }
		public Int32? CurrentEpisode { get; set; }

		public virtual ICollection<UserTitle> UserTitles { get; set; }
	}
}