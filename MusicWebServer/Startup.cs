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
using MusicBot;

namespace MusicWebServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		
		public void ConfigureServices(IServiceCollection services)
		{
			var _Bot = new Bot();
			var _SpotyManager = new SpotifyManager();

			services.AddSingleton(_Bot);

			services.AddSingleton(_SpotyManager);

			services.AddSingleton(_SpotyManager);

			//services.AddSingleton(new Handlers(_Bot, _SpotyManager));

			services.AddControllers();
		}
		
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapControllers();
				endpoints.MapControllerRoute(
					"default",
					"/{controller=Home}/{action=Index}/{id?}"
				);
			});
		}
	}
}