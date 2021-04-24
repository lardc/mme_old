using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace SCME.UpdateServer
{
    /// <summary>������������� RESTful-�������</summary>
    public class Startup
    {
        public const string LOGS_DIRECTORY = "Logs";

        public Startup(IConfiguration configuration) //������ RESTful-�������
        {
            if (!Directory.Exists(LOGS_DIRECTORY))
                Directory.CreateDirectory(LOGS_DIRECTORY);
            Configuration = configuration;
        }

        /// <summary>������������</summary>
        private IConfiguration Configuration
        {
            get;
        }

        public void ConfigureServices(IServiceCollection services) //��������� RESTful-�������
        {
            services.AddControllers();
            services.AddOptions();
            services.Configure<UpdateDataConfig>(Configuration.GetSection("UpdateDataConfig"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) //��������� ��������� � �������� �����
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseExceptionHandler("/Update/Error");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
