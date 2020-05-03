using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VkNet;
using VkNet.Model;
using VkNet.Abstractions;
using AniMediaNotifier.Data;

namespace AniMediaNotifier
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
			services.AddSingleton<IVkApi>(provider =>
			{
				VkApi api = new VkApi();
				api.Authorize(new ApiAuthParams()
				{
					AccessToken = Configuration["AccessToken"], Code = Configuration["Code"]
				});

				return api;
			});

			services.AddDbContextPool<AniMediaNotifierDbContext>(builder =>
				builder.UseInMemoryDatabase("AniMediaNotifier"));

			services.AddControllers().AddNewtonsoftJson();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}