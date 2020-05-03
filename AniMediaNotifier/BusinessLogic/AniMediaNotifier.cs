using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;
using AniMediaNotifier.Core;
using AniMediaNotifier.Data;

namespace AniMediaNotifier
{
	internal class AniMediaNotifier : IDisposable
	{
		private readonly IVkApi _api;
		private readonly ILogger _logger;
		private readonly AniMediaObserver _observer;
		private readonly AniMediaNotifierDbContext _context;

		public AniMediaNotifier(IVkApi api, ILogger logger, AniMediaObserver observer,
			AniMediaNotifierDbContext context)
		{
			_api = api;
			_logger = logger;
			_observer = observer;
			_context = context;
		}

		private async void OnTitleChanged(Object sender, TitleChangedEventArgs e)
		{
			List<User> users = _context.UserTitles.Where(ut => ut.TitleId == e.TitleId).Select(ut => ut.User).ToList();

			IEnumerable<Task> tasks = users.Select(u => SendMessageSafe(u, e));

			await Task.WhenAll(tasks);
		}

		private async Task SendMessageSafe(User user, TitleChangedEventArgs e)
		{
			try
			{
				await _api.Messages.SendAsync(new MessagesSendParams()
				{
					PeerId = user.PeerId,
					Message = $"{e.TitleName}: new {e.CurrentEpisode} episode!"
				});
			}
			catch (Exception exception)
			{
				String message = $"{DateTime.Now}: Exception: {exception.Message}\n{exception.StackTrace}";
				_logger.LogError(message);
			}
		}

		public void Dispose()
		{
			_observer.TitleChanged -= OnTitleChanged;
		}
	}
}