using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicBotV2.Services.BotServices;
using MusicBotV2.Services.Interfaces;
using Telegram.Bot;

namespace MusicBotV2.Controllers
{
	public class AuthenticationController : Controller
	{
		private readonly IMusicService _MusicService;
		private readonly ITelegramBotClient _BotClient;

		public AuthenticationController(IMusicService musicService, BotService botService)
		{
			_MusicService = musicService;
			_BotClient = botService.BotClient;
		}

		public IActionResult Hello() => Content("Hello from MusicFlow");

		// http://localhost:5000/Authentication?code=KZws3Qz6EVkfN&state=12345+465
		public async Task<IActionResult> Index(string code, string state)
		{
			if (string.IsNullOrWhiteSpace(code) || state is null)
				return BadRequest("Нет параметра(ов) для авторизации.");

			var splitted = state.Split('+').Select(long.Parse).ToList();
			long chatId = splitted[0], hostId = splitted[1];


			await _MusicService.AuthenticationFromRawTokenAsync(code, chatId, hostId);

			var chatMember = await _BotClient.GetChatMemberAsync(chatId, hostId).ConfigureAwait(false);
			var hostName = chatMember.User.Username;

			await _BotClient.SendTextMessageAsync(chatId,$"Аккаунт для воспроизведения установлен:\nХостом является {hostName}.");

			var chat = await _BotClient.GetChatAsync(chatId);

			return Content($"В чат '{chat.Title}' был установлен хост.");
		}
	}
}
