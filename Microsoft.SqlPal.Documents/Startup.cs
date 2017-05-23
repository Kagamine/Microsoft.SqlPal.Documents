using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.SqlPal.Documents
{
    public class Startup
    {
        public static IConfiguration Config;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfiguration(out Config);
            services.AddMvc();
            services.AddTransient<Lib.RazorViewToStringRenderer>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.Run(async (context) =>
            {
                var endpoint = context.Request.Path.ToString();
                var splited = endpoint.Split('/');
                if (endpoint.EndsWith("/"))
                    endpoint += "index.md";
                var toc = Lib.GitHub.RenderTocMd();
                var content = Lib.GitHub.FilterMarkdown(Lib.GitHub.GetRawFile(endpoint));
                var render = context.RequestServices.GetRequiredService<Lib.RazorViewToStringRenderer>();
                await context.Response.WriteAsync(await render.RenderViewToStringAsync("Index", new Models.Page
                {
                    Toc = toc,
                    Content = content,
                    Endpoint = endpoint,
                    Nav = JsonConvert.DeserializeObject<IDictionary<string, string>>(File.ReadAllText(Path.Combine(Config["Docs:Path"], "nav.json"))),
                    Path = context.Request.Path.ToString()
                }));
            });
        }
    }
}
