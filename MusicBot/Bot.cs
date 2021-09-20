using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;

namespace MusicBot
{
    public static class Bot
    {
        private static TelegramBotClient? _botClient;
        private static User? _me;
        private static CancellationTokenSource? _cts;

        public static bool IsStarted { get; private set; }

        public static async void StartBot()
        {
            if (IsStarted)
                return;

            _botClient = new TelegramBotClient(Configuration.Token);
            _me = await _botClient.GetMeAsync();
            _cts = new CancellationTokenSource();
            
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            _botClient.StartReceiving(
                new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
                _cts.Token
            );

            Console.WriteLine("Bot started successfully\nPress any key to stop...");
            IsStarted = true;
        }

        public static void StopBot()
        {
            if (!IsStarted)
                throw new TaskCanceledException("Can't stop bot when it isn't started");
            
            // Send cancellation request to stop bot
            _cts?.Cancel();

            Console.WriteLine("Bot stopped");
            IsStarted = false;
        }

        public static async void SendMessageAsync(ITelegramBotClient botClient, Update update, long chatId, string msg)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: msg,
                replyToMessageId: update.Message.MessageId
            );
        }
    }
}

