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

		// http://localhost:5000/Authentication?code=KZws3Qz6EVkfN&state=12345
		public async Task<IActionResult> Index(string code, long? state)
		{
			if (string.IsNullOrWhiteSpace(code) || state is null)
				return BadRequest("Нет параметра(ов) для авторизации.");

			await _MusicService.AuthenticationFromRawTokenAsync(code, state.Value);

			await _BotClient.SendTextMessageAsync(state.Value,"Аккаунт для воспроизведения установлен.");

			var chat = await _BotClient.GetChatAsync(state);

			return Content($"В чат {chat.Title} был установлен хост.");
		}
	}
}
