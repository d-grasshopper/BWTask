﻿using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Health3.StartupConfig
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Health3 API",
                    Description = "Health3 Endpoint",
                    TermsOfService = "None",
                    Contact = new Contact()
                    { Name = "Health3 Software", Email = "contact@bw.com", Url = "www.bw.com" }
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
        }
        private static string GetXmlCommentsPath()
        {
            var app = System.AppContext.BaseDirectory;
            return System.IO.Path.Combine(app, "Health3.xml");
        }
    }
}
