using System;
using AniMediaNotifier.Core;

namespace AniMediaNotifier
{
	internal class TitleChangedEventArgs : EventArgs
	{
		public Int32 TitleId { get; }
		public String TitleName { get; }
		public TitleStatus TitleStatus { get; }
		public Int32? CurrentEpisode { get; }

		public TitleChangedEventArgs(Int32 titleId, String titleName, TitleStatus titleStatus, Int32? currentEpisode)
		{
			TitleId = titleId;
			TitleName = titleName;
			TitleStatus = titleStatus;
			CurrentEpisode = currentEpisode;
		}
	}
}