using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AniMediaNotifier.Core;
using AniMediaNotifier.Data;

namespace AniMediaNotifier
{
	internal class AniMediaObserver
	{
		private readonly List<Title> _titles;
		private readonly AniMediaHtmlParser _htmlParser;
		
		private readonly AniMediaNotifierDbContext _context;
		private readonly ILogger _logger;

		public event EventHandler<TitleAddedEventArgs> TitleAdded;
		public event EventHandler<TitleChangedEventArgs> TitleChanged;

		public AniMediaObserver(AniMediaNotifierDbContext context, ILogger logger)
		{
			_context = context;
			_logger = logger;

			_titles = new List<Title>();
			_htmlParser = new AniMediaHtmlParser();
		}

		public async Task Check()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			_logger.LogInformation($"{DateTime.Now}: Check site for new titles started.");

			List<Title> titles = await _htmlParser.GetTitlesAsync();
			List<Task> tasks = titles.Except(_titles).Select(title => Task.Run(() => AddOrUpdate(title))).ToList();

			await Task.WhenAll(tasks);

			_titles.Clear();
			_titles.AddRange(titles);
			
			stopwatch.Stop();

			_logger.LogInformation($"{DateTime.Now}: Checking ended. Total time: {stopwatch.ElapsedMilliseconds} ms.");
		}

		private async Task AddOrUpdate(Title title)
		{
			Title dto = await _context.Titles.SingleOrDefaultAsync(t => t.Name == title.Name);

			if (dto == null)
			{
				dto = new Title
				{
					Id = 0, Name = title.Name, Status = title.Status, CurrentEpisode = title.CurrentEpisode
				};

				_context.Titles.Add(dto);
				_context.SaveChanges();

				_logger.LogInformation($"{DateTime.Now}: {dto.Name} was added to database.");

				TitleAdded?.Invoke(this, new TitleAddedEventArgs(dto.Name));

				return;
			}

			if (dto.Status == TitleStatus.Completed || dto.CurrentEpisode == title.CurrentEpisode) return;

			dto.Status = title.Status;
			dto.CurrentEpisode = title.CurrentEpisode;

			_context.Titles.Update(dto);
			_context.SaveChanges();

			_logger.LogInformation($"{DateTime.Now}: {dto.Name} was updated.");

			TitleChanged?.Invoke(this, new TitleChangedEventArgs(dto.Id, dto.Name, dto.Status, dto.CurrentEpisode));
		}
	}
}