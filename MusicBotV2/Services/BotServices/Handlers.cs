using System;
using System.Threading;
using System.Threading.Tasks;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MusicBotV2.Services.BotServices
{
	public class Handlers
	{
		private const string WrongLinkMessage = "Неверная ссылка";
		private const string ScsMessage = "Ссылка успешно обработана";
		private const string ErrMessage = "Ошибка добавления";


		private readonly IMusicService _MusicService;

		public Handlers(IMusicService musicService)
		{
			_MusicService = musicService;
		}

		public static Task HandleErrorAsync(ITelegramBotClient botClient,
			Exception exception,
			CancellationToken cancellationToken)
		{
			var errorMessage = exception switch {
				ApiRequestException apiRequestException =>
					$"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ //default
					=> exception.ToString()
			};

			Console.WriteLine(errorMessage);
			return Task.CompletedTask;
		}

		public async Task HandleUpdateAsync(ITelegramBotClient botClient,
			Update update,
			CancellationToken cancellationToken)
		{
			if (update.Type != UpdateType.Message)
				return;
			if (update.Message.Type != MessageType.Text)
				return;

			var chatId = update.Message.Chat.Id;
			string current_message = update.Message.Text;
			Console.WriteLine($"Received a '{current_message}' message in chat {chatId}.");

			if (current_message.ToLower() == "host")
			{
				BotService.SendMessageAsync(
					botClient,
					update,
					"Follow the link: " + SpotifyManager.BuildAuthenticationLink(chatId)
					);
				return;
			}

			string url = update.Message.Text; // URL to the song, that client send
			var spotifyTrackId = await  LinkHandler.HandleLinkOrDefaultAsync(url);

			string msg = string.Empty;

			if (spotifyTrackId is null)
				msg = WrongLinkMessage;
			else
			{
				var isAdded = await _MusicService.AddToQueueAsync(spotifyTrackId, chatId);

				msg = isAdded ? ScsMessage : ErrMessage;
			}

			BotService.SendMessageAsync(
					botClient,
					update,
					msg
				);
		}
	}
}
