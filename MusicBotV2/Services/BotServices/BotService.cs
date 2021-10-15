using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicBotV2.Services.Static;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace MusicBotV2.Services.BotServices
{
	public class BotService
	{
		private readonly Handlers _Handlers;
		private readonly ILogger<BotService> _Logger;
		private TelegramBotClient _BotClient;
		private CancellationTokenSource _Cts;

		public bool IsStarted { get; private set; }

		public BotService(Handlers handlers, ILogger<BotService> logger = null)
		{
			_Handlers = handlers;
			_Logger = logger;
		}

		public void StartBot()
		{
			if (IsStarted)
				return;

			_BotClient = new TelegramBotClient(ConfigurationService.Token);
			_Cts = new CancellationTokenSource();

			// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
			_BotClient.StartReceiving(
				new DefaultUpdateHandler(_Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
				_Cts.Token
			);

			_Logger?.LogInformation("Bot started successfully\nPress any key to stop...");
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

		public static async void SendMessageAsync(ITelegramBotClient botClient, Update update, string msg)
		{
			await botClient.SendTextMessageAsync(
				chatId: update.Message.Chat.Id,
				text: msg,
				replyToMessageId: update.Message.MessageId
			);
		}
	}
}
