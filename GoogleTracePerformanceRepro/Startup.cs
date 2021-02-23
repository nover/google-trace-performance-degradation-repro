using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
using GoogleTracePerformanceRepro.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GoogleTracePerformanceRepro
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
            services.AddControllers();
            if (Configuration["Google:EnableTrace"] == "True")
            {
                Console.WriteLine("adding google trace!");
                services.AddGoogleTrace(
                    options =>
                    {
                        options.ProjectId = Configuration["Google:ProjectId"];
                        options.Options = TraceOptions.Create(
                            bufferOptions: BufferOptions.TimedBuffer(TimeSpan.FromSeconds(5.5)),
                            qpsSampleRate: 1D,
                            retryOptions: RetryOptions.NoRetry(ExceptionHandling.Ignore)
                        );
                        options.TraceFallbackPredicate = TraceDecisionPredicate.Create(request =>
                        {
                            // Do not trace OPTIONS 
                            var isOptionsCall = request.Method.ToLowerInvariant().Equals("options");

                            // Do not trace our monitoring routes
                            var isMonitoringRoute =
                                request.Path.Equals(PathString.FromUriComponent("/"));

                            return !(isOptionsCall || isMonitoringRoute);
                        });
                    });
            }

            services.AddHttpClient<IGetStuffService, GetStuffService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Configuration["Google:EnableTrace"] == "True")
            {
                Console.WriteLine("using google trace");
                app.UseGoogleTrace();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}