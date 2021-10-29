using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using MusicFlow.DAL.Context;
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
		private const string SetHostCommand = "/host";
		private const string RemoveHostCommand = "/unhost";

		private readonly IMusicService _MusicService;
		private readonly MusicFlowDb _Db;

		public Handlers(IMusicService musicService, MusicFlowDb db)
		{
			_MusicService = musicService;
			_Db = db;
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

			var current_user = update.Message.From.Id;
			var chatId = update.Message.Chat.Id;
			string current_message = update.Message.Text;
			Console.WriteLine($"Received a '{current_message}' message in chat {chatId} from user {current_user}.");

			var chat = await _Db.Chats.FirstOrDefaultAsync(c => c.TelegramChatId == chatId,
				cancellationToken).ConfigureAwait(false);

			ChatMember host = null;

			if (chat?.HostUserId is not null)
				host = await botClient.GetChatMemberAsync(chatId, chat.HostUserId.Value, cancellationToken);

			//SetHost handling.
			if (current_message.ToLower() == SetHostCommand) {
				if (host is not null) {
					if (host.User.Id != current_user)
						await botClient.SendTextMessageAsync(chatId,
							"В данный момент команда недоступна. " +
							$"Запросите {host.User.Username} отказаться от данной роли.",
							replyToMessageId: update.Message.MessageId,
							cancellationToken: cancellationToken
							).ConfigureAwait(false);
					else
						await botClient.SendTextMessageAsync(chatId,
							"В данный момент команда недоступна. Вы являетесь хостом.",
							replyToMessageId: update.Message.MessageId,
							cancellationToken: cancellationToken
							).ConfigureAwait(false);
					return;
				}

				await botClient.SendTextMessageAsync(
					chatId,
					"Перейдите по ссылке: " + SpotifyManager.BuildAuthenticationLink(chatId, current_user),
					replyToMessageId: update.Message.MessageId,
					cancellationToken: cancellationToken
				).ConfigureAwait(false);
				return;
			}

			//RemoveHost handling.
			if (current_message.ToLower() == RemoveHostCommand) {
				if (host is null) {
					await botClient.SendTextMessageAsync(chatId,
							"В данный момент команда недоступна. " +
							"Нет аккаунта для воспроизведения.\n" +
							$"Введите {SetHostCommand} чтобы стать хостом.",
							replyToMessageId: update.Message.MessageId,
						cancellationToken: cancellationToken
					).ConfigureAwait(false);
					return;
				}

				if (host.User.Id == current_user) {
					chat.Token = null;
					chat.HostUserId = null;
					chat.RefreshToken = null;
					chat.ExpiresIn = null;
					chat.CreatedAt = null;

					_Db.Chats.Update(chat);
					await _Db.SaveChangesAsync(cancellationToken);

					await botClient.SendTextMessageAsync(
						chatId,
						"Аккаунт воспроизведения сброшен.",
						replyToMessageId: update.Message.MessageId,
						cancellationToken: cancellationToken
					).ConfigureAwait(false);
					return;
				}
				await botClient.SendTextMessageAsync(
					chatId,
					"Вам недоступна данная комманда.",
					replyToMessageId: update.Message.MessageId,
					cancellationToken: cancellationToken
				).ConfigureAwait(false);
				return;
			}

			//Link handling.
			if (Regex.IsMatch(current_message, "https?://.+")) {
				if (chat?.HostUserId is null) {
					await botClient.SendTextMessageAsync(
						chatId,
						$"Аккаунт для воспроизведения не установлен. Пожалуйста, введите '{SetHostCommand}' для установки.",
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
				//Console.WriteLine("spotifyTrackId: {0}", spotifyTrackId);

				var isAdded = await _MusicService.AddToQueueAsync(spotifyTrackId, chatId);

				string msg = isAdded is null ? $"У хоста {host.User.Username} нет активного Spotify клиента." :
					(isAdded.Value ? ScsTrackMessage : ErrTrackMessage);

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
