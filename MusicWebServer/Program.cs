using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicBot;

namespace MusicWebServer
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			Configuration.Initialize();
			var bot = host.Services.GetService(typeof(Bot)) as Bot;

			try {
				bot!.StartBot();
			}
			catch (Exception e) {
				Console.WriteLine(e.Message);
			}

			await host.RunAsync();
			bot!.StopBot();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
				  .ConfigureWebHostDefaults(webBuilder =>
				  {
					  webBuilder.UseStartup<Startup>();
				  });
	}
}