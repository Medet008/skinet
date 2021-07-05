using System.IO;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace API
{
  public class Startup
  {
    private readonly IConfiguration _configuration;
    public Startup(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    // Это основа соглашений, и потому что это называется "Настроить службы разработки"
    // Все, что находится внутри, будет выполнено / добавлено в наш контейнер для внедрения зависимостей
    // когда мы работаем в режиме разработки.
    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      services.AddDbContext<StoreContext>(x =>
        x.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));

      services.AddDbContext<AppIdentityDbContext>(x => 
        x.UseSqlite(_configuration.GetConnectionString("IdentityConnection")));

        ConfigureServices(services);
    }

    public void ConfigureProductionServices(IServiceCollection services)
    {
// Настройте здесь MYSQL или просто SQL-сервер
      // services.AddDbContext<StoreContext>(x =>
      //   x.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));

      // services.AddDbContext<AppIdentityDbContext>(x => 
      //   x.UseSqlite(_configuration.GetConnectionString("IdentityConnection")));

        ConfigureServices(services);
    }
    

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAutoMapper(typeof(MappingProfiles));

      services.AddControllers();

      // Has it own methods now
      // services.AddDbContext<StoreContext>(x =>
      // x.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));

      // services.AddDbContext<AppIdentityDbContext>(x => 
      //   x.UseSqlite(_configuration.GetConnectionString("IdentityConnection")));

      services.AddSingleton<IConnectionMultiplexer>(c =>
      {
        var configuration = ConfigurationOptions.Parse(_configuration
            .GetConnectionString("Redis"), true);
        return ConnectionMultiplexer.Connect(configuration);
      });

      services.AddApplicationServices();
      services.AddIdentityServices(_configuration);
      services.AddSwaggerDocumentation();

      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
        {
          policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
        });
      });

    }

// Этот метод вызывается средой выполнения. Используйте этот метод для настройки конвейера HTTP-запросов.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseMiddleware<ExceptionMiddleware>();
      
// {0} - это код ошибки
      app.UseStatusCodePagesWithReExecute("/errors/{0}");

      app.UseHttpsRedirection();

      app.UseRouting();

// использует файл wwwroot
      app.UseStaticFiles();

      // потому что мы переместили изображения из папки wwwroot
      // нам нужно указать API, где искать изображения.
      app.UseStaticFiles(new StaticFileOptions 
      {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "Content")
        ),
        RequestPath = "/content"
      });

      app.UseCors("CorsPolicy");

      app.UseAuthentication();
      
      app.UseAuthorization();

      app.UseSwaggerDocumentation();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        //endpoints.MapFallbackToController("Index", "Fallback");
      });
    }
  }
}
