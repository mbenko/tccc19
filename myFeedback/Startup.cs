using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using myFeedback.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace myFeedback
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Add SQL Data Store
            services.AddDbContext<myDataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("myDataContext")));

            // Add Cosmos Store
            var client = new CosmosClient(Configuration.GetConnectionString("myCosmos"));
            CosmosDatabase db = client.Databases.CreateDatabaseIfNotExistsAsync("TCCC19").Result;
            CosmosContainer myFeedback = db.Containers.CreateContainerIfNotExistsAsync("Feedback", "/Session", 400).Result;

            services.AddSingleton<CosmosContainer>(myFeedback);

            // Add Azure Queues
            CloudStorageAccount AzAccount = CloudStorageAccount.Parse(Configuration.GetConnectionString("myStorage"));
            CloudQueueClient azQClient = AzAccount.CreateCloudQueueClient();
            CloudQueue azQueue = azQClient.GetQueueReference("new-feedback");
            azQueue.CreateIfNotExistsAsync();

            services.AddSingleton<CloudQueue>(azQueue);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
