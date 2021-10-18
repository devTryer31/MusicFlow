using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicBotV2.Services;
using MusicBotV2.Services.BotServices;
using MusicBotV2.Services.Static;

namespace MusicBotV2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ConfigurationService.Initialize();
			var host = CreateHostBuilder(args).Build();

			var bot = host.Services.GetService(typeof(BotService)) as BotService;

			bot!.StartBot();

			host.Run();
			bot.StopBot();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
				  .ConfigureWebHostDefaults(webBuilder =>
				  {
					  webBuilder.UseStartup<Startup>();
				  });
	}
}
