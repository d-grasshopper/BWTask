﻿using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Health2.StartupConfig
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
                    Title = "Health2 API",
                    Description = "Health2 Endpoint",
                    TermsOfService = "None",
                    Contact = new Contact()
                    { Name = "Health2 Software", Email = "contact@bw.com", Url = "www.bw.com" }
                });
                c.IncludeXmlComments(GetXmlCommentsPath());
            });
        }
        private static string GetXmlCommentsPath()
        {
            var app = System.AppContext.BaseDirectory;
            return System.IO.Path.Combine(app, "Health2.xml");
        }
    }
}
