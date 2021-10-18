using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MusicBotV2.Data;
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
		private const string ScsTrackMessage = "Трек успешно добавлен.";
		private const string ErrTrackMessage = "Ошибка добавления трека.";


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
			//Console.WriteLine($"Received a '{current_message}' message in chat {chatId}.");


			//Host handling.
			if (current_message.ToLower() == "хост") {
				
				var isHostSet = InMemoryDatabaseTest.Data.ContainsKey(chatId);
				if (isHostSet)
				{
					await botClient.SendTextMessageAsync(chatId,
						"В данный момент команда недоступна. Запросите хост-аккаунт отказаться от данной роли.",
						cancellationToken: cancellationToken
						).ConfigureAwait(false);
					return;
				}

				await botClient.SendTextMessageAsync(
					chatId,
					"Перейдите по ссылке: " + SpotifyManager.BuildAuthenticationLink(chatId),
					replyToMessageId: update.Message.MessageId,
					cancellationToken: cancellationToken
				).ConfigureAwait(false);
				;
				return;
			}

			//Link handling.
			if (Regex.IsMatch(current_message, "https?://.+")) {
				if (!InMemoryDatabaseTest.Data.ContainsKey(chatId)) {
					await botClient.SendTextMessageAsync(
						chatId,
						"Аккаунт для воспроизведения не установлен. Пожалуйста, введите 'хост' для установки.",
						replyToMessageId: update.Message.MessageId,
						cancellationToken: cancellationToken
					).ConfigureAwait(false);
					return;
				}

				var spotifyTrackId = await LinkHandler.HandleLinkOrDefaultAsync(current_message);

				if (spotifyTrackId is null) {
					await botClient.SendTextMessageAsync(
						chatId,
						WrongLinkMessage,
						replyToMessageId: update.Message.MessageId,
						cancellationToken: cancellationToken
					).ConfigureAwait(false);
					return;
				}

				Console.WriteLine("spotifyTrackId: {0}", spotifyTrackId);
				var isAdded = await _MusicService.AddToQueueAsync(spotifyTrackId, chatId);

				string msg = isAdded ? ScsTrackMessage : ErrTrackMessage;

				await botClient.SendTextMessageAsync(
						chatId,
						msg,
						replyToMessageId: update.Message.MessageId,
						cancellationToken: cancellationToken
				).ConfigureAwait(false);
			}
		}
	}
}
