using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicBotV2.Services.BotServices;
using MusicBotV2.Services.Interfaces;
using Telegram.Bot;

namespace MusicBotV2.Controllers
{
	[Route("")]
	[Route("authentication")]
	public class AuthenticationController : Controller
	{
		private const string StartBackgroundColor = "#E8E8E8";
		private const string SuccessBackgroundColor = "#90EE90";
		private const string ErrorBackgroundColor = "#FFA07A";

		private readonly IMusicService _MusicService;
		private readonly ITelegramBotClient _BotClient;

		public AuthenticationController(IMusicService musicService, BotService botService)
		{
			_MusicService = musicService;
			_BotClient = botService.BotClient;
		}
		
		[Route("")]
		[Route("index")]
		// http://localhost:5000/Authentication?code=KZws3Qz6EVkfN&state=12345+465
		public async Task<IActionResult> Index(string code, string state)
		{
			if (string.IsNullOrWhiteSpace(code) && string.IsNullOrEmpty(state))
				return View("Index", ("Hello from MusicFlow!", StartBackgroundColor));

			if (string.IsNullOrWhiteSpace(code) || string.IsNullOrEmpty(state))
				return View("Index", ("Ошибка: Нет параметра(ов) для авторизации.", ErrorBackgroundColor));

			var splitted = state.Split('+').Select(long.Parse).ToList();
			long chatId = splitted[0], hostId = splitted[1];


			bool isHostSettled = await _MusicService.AuthenticationFromRawTokenAsync(code, chatId, hostId);

			if (!isHostSettled)
				return View("Index", ("Хост для текущего чата уже установлен. Отказ в доступе.", ErrorBackgroundColor));

			var chatMember = await _BotClient.GetChatMemberAsync(chatId, hostId).ConfigureAwait(false);
			var hostName = chatMember.User.Username;

			await _BotClient.SendTextMessageAsync(chatId, $"Аккаунт для воспроизведения установлен:\nХостом является {hostName}.");

			var chat = await _BotClient.GetChatAsync(chatId);

			return View("Index", ($"В чат '{chat.Title}' был установлен хост.", SuccessBackgroundColor));
		}
	}
}
