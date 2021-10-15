using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicBot;

namespace MusicWebServer.Controllers
{
	public class SpotifyController : Controller
	{
		private readonly SpotifyManager _SpotifyManager;

		public SpotifyController(SpotifyManager spotifyManager)
		{
			_SpotifyManager = spotifyManager;
		}

		public async Task<IActionResult> GetToken(string stringRequest)
		{
			var res = stringRequest.Split('&');

			string token = res[0].Split('=')[1];
			string chatID = res[1].Split('=')[1];

			await _SpotifyManager.Launch(token);

			return StatusCode(200);
		}
	}
}
