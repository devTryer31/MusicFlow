using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MusicWebServer.Controllers
{
	public class SpotifyController : Controller
	{
		public IActionResult GetToken(string stringRequest)
		{
			return View();
		}
	}
}
