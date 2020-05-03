using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AniMediaNotifier.Core;
using HtmlAgilityPack;

namespace AniMediaNotifier
{
	internal class AniMediaHtmlParser
	{
		private readonly String _html;
		private readonly String _tagTitleInfo;
		private readonly String _tagTitleName;
		private readonly String _tagTitleStatus;

		private readonly HtmlWeb _htmlLoader;

		public AniMediaHtmlParser()
		{
			_html = @"https://online.animedia.tv/";
			_tagTitleInfo = "//div[@class='widget__new-series__item__info']";
			_tagTitleName = "a[@class='h4 widget__new-series__item__title']";
			_tagTitleStatus = "div[@class='widget__new-series__item__status']";

			_htmlLoader = new HtmlWeb();
		}

		public async Task<List<Title>> GetTitlesAsync()
		{
			HtmlDocument htmlDocument = await Task.Run(() => _htmlLoader.Load(_html));

			HtmlNodeCollection nodes = await Task.Run(() =>
				htmlDocument.DocumentNode.SelectNodes(_tagTitleInfo));

			List<Title> titles = nodes.Select(ParseTitleInfo).ToList();

			return titles;
		}

		private Title ParseTitleInfo(HtmlNode htmlNode)
		{
			String name = htmlNode.SelectSingleNode(_tagTitleName).InnerText;
			String statusData = htmlNode.SelectSingleNode(_tagTitleStatus).InnerText;

			Int32? currentEpisode = null;

			TitleStatus status = ParseTitleStatus(statusData);

			if (status == TitleStatus.Ongoing) ParseEpisodeData(statusData, out currentEpisode);

			return new Title {Name = name, Status = status, CurrentEpisode = currentEpisode};
		}

		private static TitleStatus ParseTitleStatus(String input)
		{
			return input == "Релиз завершён" ? TitleStatus.Completed : TitleStatus.Ongoing;
		}

		private static void ParseEpisodeData(String input, out Int32? currentEpisode)
		{
			String[] data = input.Split(" ");

			if (data.Length != 4)
				throw new InvalidDataException("Input does not match to pattern \"Серия [X] из [Y]\".");

			currentEpisode = null;

			if (Int32.TryParse(data[1], out Int32 result)) currentEpisode = result;
		}
	}
}