using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using MusicFlow.DAL.Context;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace MusicBotV2.Services.BotServices
{
	public class BotService
	{
		private readonly Handlers _Handlers;
		private readonly ILogger<BotService> _Logger;
		public TelegramBotClient BotClient { get; set; }
		private CancellationTokenSource _Cts;

		public bool IsStarted { get; private set; }

		public BotService(ILogger<BotService> logger, IMusicService musicService, MusicFlowDb db)
		{//it construct twice while scoped.
			_Handlers = new Handlers(musicService, db);
			_Logger = logger;
		}

		public void StartBot()
		{
			if (IsStarted)
				return;

			BotClient = new TelegramBotClient(ConfigurationService.Token);
			_Cts = new CancellationTokenSource();

			// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
			BotClient.StartReceiving(
				new DefaultUpdateHandler(_Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
				_Cts.Token
			);

			_Logger?.LogInformation("Bot started successfully\n");
			IsStarted = true;
		}

		public void StopBot()
		{
			if (!IsStarted)
				throw new TaskCanceledException("Can't stop bot when it isn't started");

			// Send cancellation request to stop bot
			_Cts?.Cancel();

			_Logger?.LogInformation("Bot stopped");
			IsStarted = false;
		}
	}
}
