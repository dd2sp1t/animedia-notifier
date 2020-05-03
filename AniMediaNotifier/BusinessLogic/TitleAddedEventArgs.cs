using System;

namespace AniMediaNotifier
{
	internal class TitleAddedEventArgs : EventArgs
	{
		public String TitleName { get; }

		public TitleAddedEventArgs(String titleName)
		{
			TitleName = titleName;
		}
	}
}