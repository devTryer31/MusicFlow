using System;
using SpotifyAPI.Web.Http;

namespace MusicBotV2.Services.Loggers
{
	public class SpotifyHTTPLogger : IHTTPLogger
	{
		private void PrintLine() => Console.WriteLine("========================================");

		public void OnRequest(IRequest request)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			PrintLine();
			Console.WriteLine("Got request:{0}", request.Body);
			PrintLine();
			Console.ForegroundColor = ConsoleColor.White;
		}

		public void OnResponse(IResponse response)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			PrintLine();
			Console.WriteLine("Got response:{0}", response.Body);
			PrintLine();
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}
