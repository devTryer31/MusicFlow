using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;

namespace MusicBot
{
	public class Bot
	{
		private TelegramBotClient _botClient;
		private User _me;
		private CancellationTokenSource _cts;

		public bool IsStarted { get; private set; }

		public Bot()
		{
		}

		public async void StartBot()
		{
			if (IsStarted)
				return;

			_botClient = new TelegramBotClient(Configuration.Token);
			_me = await _botClient.GetMeAsync();
			_cts = new CancellationTokenSource();

			// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
			_botClient.StartReceiving(
				 //new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
				 //_cts.Token
			);

			Console.WriteLine("Bot started successfully\nPress any key to stop...");
			IsStarted = true;
		}

		public void StopBot()
		{
			if (!IsStarted)
				throw new TaskCanceledException("Can't stop bot when it isn't started");

			// Send cancellation request to stop bot
			_cts?.Cancel();

			Console.WriteLine("Bot stopped");
			IsStarted = false;
		}

		public async void SendMessageAsync(ITelegramBotClient botClient, Update update, long chatId, string msg)
		{
			await botClient.SendTextMessageAsync(
				 chatId: chatId,
				 text: msg,
				 replyToMessageId: update.Message.MessageId
			);
		}
	}
}

