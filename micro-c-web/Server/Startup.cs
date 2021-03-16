using Hangfire;
using Hangfire.SqlServer;
using micro_c_web.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace micro_c_web.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var secrets = Configuration.GetSection(Secrets.SecretsName).Get<Secrets>();
            var connection = $"Server=db;Database=master;User=sa;Password={secrets?.DbPassword ?? ""};";

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connection)
            );

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSwaggerGen();

            services.AddHangfire(configuration => configuration
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    $"Server=db;Database=master;User=sa;Password={secrets?.DbPassword ?? ""};",
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    })
            );

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "cache", "default" };
                options.WorkerCount = 1;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, IBackgroundJobClient backgroundJobs)
        {
            context.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroCWeb V1");
                });

                app.UseHangfireDashboard("/hangfire", new DashboardOptions()
                {
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            Hangfire.BackgroundJob.Enqueue<CacheProcessor>(proc => proc.PrimeStaleItems());

            RecurringJob.AddOrUpdate<CacheProcessor>("prime-cache", proc => proc.PrimeStaleItems(), Cron.Hourly(25), queue: "cache");
            RecurringJob.AddOrUpdate<CacheProcessor>("pump-cache", proc => proc.ProcessAllCached(), Cron.Hourly(40), queue: "cache");


            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
