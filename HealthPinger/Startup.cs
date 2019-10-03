using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HealthPinger.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HealthPinger.StartupConfig;
using HealthPinger.Helpers;
using HealthPinger.Services;
using Microsoft.IdentityModel.Protocols.WsFederation;

namespace HealthPinger
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
            IEnumerable<KeyValuePair<string, string>> serviceLocations;
            string elasticUri;
            string elasticAuth;
            string isKubernetes;
            try
            {
                isKubernetes = Environment.GetEnvironmentVariable("ASPNETCORE_KUBERNETES");
            }
            catch (Exception e)
            {
                isKubernetes = "";
            }

            if (isKubernetes == "true")
            {
                serviceLocations = GetServiceSettings(true);
                var elasticSettings = Configuration.GetSection("KubernetesElastic");
                elasticUri = elasticSettings["Uri"];
                elasticAuth = elasticSettings["Auth"];
            }
            else
            {
                serviceLocations = GetServiceSettings(false);
                var elasticSettings = Configuration.GetSection("Elastic");
                elasticUri = elasticSettings["Uri"];
                elasticAuth = elasticSettings["Auth"];
            }

            Dictionary<string, string> serviceDict = new Dictionary<string, string>();
            foreach (var service in serviceLocations)
            {
                if (service.Key.Contains("Health"))
                {
                    serviceDict.Add(service.Key, service.Value);
                }
            }

            services.AddSingleton<IPingHealthEndpoints>(new HealthPingService(new HttpWrapper(), serviceDict));
            services.AddSingleton<IPostToElastic>(new ElasticService(new HttpWrapper(), elasticUri, elasticAuth));
            services.AddSingleton<ICheckHealthAndPost, HealthChecker>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.ConfigureSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Health0 Api v1");
            });
        }

        public IEnumerable<KeyValuePair<string, string>> GetServiceSettings(bool isKubernetes)
        {
            IEnumerable<KeyValuePair<string, string>> serviceLocations;
            if (isKubernetes)
            {
                serviceLocations = Configuration.GetSection("KubernetesServices").AsEnumerable();

            }
            else
            {
                serviceLocations = Configuration.GetSection("Services").AsEnumerable();
            }

            return serviceLocations;
        }
    }
}