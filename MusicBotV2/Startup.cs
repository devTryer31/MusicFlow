using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicBotV2.Services;
using MusicBotV2.Services.BotServices;
using MusicBotV2.Services.Interfaces;
using MusicBotV2.Services.Static;
using MusicFlow.DAL.Context;

namespace MusicBotV2
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<MusicFlowDb>(opt =>
				opt.UseSqlite(Configuration.GetConnectionString("SQLite"),
					o => o.MigrationsAssembly("MusicFlow.DAL.SQLite")),
				ServiceLifetime.Singleton);
			
			services.AddSingleton<IMusicService, SpotifyManager>();
			
			services.AddSingleton<BotService>();

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					"default",
					"/{controller=Home}/{action=Index}/{id?}"
				);
			});
		}
	}
}
