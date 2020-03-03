using Acme.ProductImport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;
using Totem.Timeline.Hosting;
using Totem.Timeline.Mvc;
using Totem.Timeline.StreamsDb.Hosting;

namespace Totem.Sample.Api
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
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
      });

      services.AddTotemRuntime();

      services.AddTimelineClient<FinancialArea>(timeline =>
      {
        timeline.AddStreamsDb("", "sample21");
      });

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddScoped<IQueryServer, QueryServer>();
      services.AddScoped<ICommandServer, CommandServer>();
      services.AddSingleton<IQueryNotifier, EmptyQueryNotifier>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc();

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });
    }
  }
}
