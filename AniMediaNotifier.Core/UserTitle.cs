using System;

namespace AniMediaNotifier.Core
{
	public class UserTitle
	{
		public Int32 UserId { get; set; }
		public Int32 TitleId { get; set; }

		public User User { get; set; }
		public Title Title { get; set; }
	}
}