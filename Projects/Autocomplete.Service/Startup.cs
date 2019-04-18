using System;
using System.IO;
using System.Reflection;
using Autocomplete.DataAccess.Repositories;
using Autocomplete.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Autocomplete.Service
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
			DataAccess.Configuration.ConnectionString = Configuration.GetConnectionString("Database");

			services.AddCors();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Autocomplete API", Version = "v1" });
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});

			services.AddSingleton(Configuration);
			services.AddScoped<IEntryRepository, EntryRepository>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseCors(x => x
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();

			var path = Configuration.GetValue<string>("APPL_PATH");
			if (path == "/")
			{
				path = string.Empty;
			}

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint($"{path}/swagger/v1/swagger.json", "Autocomplete API V1");
			});

			app.UseMvc();
		}
	}
}
