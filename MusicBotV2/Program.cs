using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicBotV2.Services.BotServices;
using MusicBotV2.Services.Static;
using MusicFlow.DAL.Context;

namespace MusicBotV2
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			ConfigurationService.Initialize();
			var host = CreateHostBuilder(args).Build();

			using var scope = host.Services.CreateScope();
			var bot = scope.ServiceProvider.GetService<BotService>();
			var dbContext = scope.ServiceProvider.GetService<MusicFlowDb>();
			var migrations = await dbContext!.Database.GetPendingMigrationsAsync();

			if (migrations.Any())
			{
				await dbContext.Database.MigrateAsync();
			}
			
			bot!.StartBot();

			await host.RunAsync();
			bot.StopBot();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
				  .ConfigureWebHostDefaults(webBuilder =>
				  {
					  webBuilder.UseStartup<Startup>();
					  webBuilder.UseIIS();
				  });
	}
}
