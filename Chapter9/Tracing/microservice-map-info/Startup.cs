using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace microservice_map_info
{
	public class Startup
	{
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			_env = env;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			string jaegerHost = Configuration.GetValue<string>("openTelemetry:jaegerHost");
			services.AddOpenTelemetryTracing(builder =>
			{
				builder
					.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_env.ApplicationName))
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					.AddJaegerExporter(options =>
					{
						options.AgentHost = jaegerHost;
					})
					.AddConsoleExporter();
			});

			services.AddScoped<DistanceInfoService>();
			services.AddHttpClient("googleApi", client =>
			{
				client.BaseAddress = new Uri(Configuration["googleDistanceApi:apiUrl"]);
			});

			services.AddControllers();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My map API", Version = "v1" });
			});

			services.AddGrpc();


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			if (_env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json","My microservice for map information.");
			});

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<DistanceInfoService>();
				endpoints.MapControllers();
			});
		}
	}
}
