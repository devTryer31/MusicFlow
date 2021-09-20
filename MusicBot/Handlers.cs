using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MusicBot
{
    internal static class Handlers
    {
        private const string WrongLinkMessage = "Неверная ссылка";
        private const string ScsMessage = "Ссылка успешно обработана";
        private const string ErrMessage = "Ошибка добавления";
        public static Task HandleErrorAsync(ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException =>
                    $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _
                    => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;
            if (update.Message.Type != MessageType.Text)
                return;
            var chatId = update.Message.Chat.Id;
            Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");

            string url = update.Message.Text; // URL to the song, that client send
            string key;
            bool success = LinkHandler.HandleSpotifyLink(url, out key);

            string msg = WrongLinkMessage;
            if (success)
            {
                bool status = await SpotifyManager.AddToQueue(url);
                if (status)
                    msg = ScsMessage;
                else
                {
                    msg = ErrMessage;
                }
            }
            else
            {
                msg = WrongLinkMessage;
            }


            if (Bot.IsStarted)
            {
                Bot.SendMessageAsync(
                    botClient: botClient,
                    update: update,
                    chatId: chatId,
                    msg: msg
                );
            }

        }
    }
}