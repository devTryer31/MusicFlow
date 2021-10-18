using System;
using Microsoft.AspNetCore.Mvc;
using MusicBotV2.Services.Interfaces;

namespace MusicBotV2.Controllers
{
	public class AuthenticationController : Controller
	{
		private readonly IMusicService _MusicService;

		public AuthenticationController(IMusicService musicService)
		{
			_MusicService = musicService;
		}

		// http://localhost:5000/callback?code=KZws3Qz6EVkfN&state=12345
		public IActionResult Index(string code, long? state)
		{
			if (string.IsNullOrWhiteSpace(code) || state is null)
				return Content($"Нет параметров для AuthenticationController");


			//var res = stringRequest.Split('&');

			//string token = res[0].Split('=')[1];
			//string chatID = res[1].Split('=')[1];

			_MusicService.AuthenticationFromRawTokenAsync(code, state.Value);

			return Content($"В чат {state} был установлен хост.");
		}
	}
}
