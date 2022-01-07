using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace RateLimit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // needed to load configuration from appsettings.json
            services.AddOptions();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Estudo Rate Limit ", Version = "v1" });
            });

            #region RateLimit

            // needed to store rate limit counters and ip rules
            // services.AddMemoryCache();


            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            //   services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));



            //services.Configure<IpRateLimitOptions>(options =>               // Defini o limite de cota por IP de Origem
            //{
            //    options.GeneralRules = new List<RateLimitRule>()            // Regra de limite de requisição
            //    {
            //        new()
            //        {
            //            Endpoint = ":/RateLimit",                              // Expressão regular para filtrar o recurso http a ser monitorado
            //            Period = "1s",                                      // Período 1s = um segundo. Use m: minuto, h: hora e d: dia
            //            Limit = 1,                                          // Total de requisições permitidas dentro do período
            //            QuotaExceededResponse = new QuotaExceededResponse   //Padronizaçãop da resposta
            //            {
            //                Content = "Too Many Requests in 1s",            //Resposta
            //                ContentType = "application/text",               // Tipo da resposta. Use application/json para retorno JSON
            //                StatusCode = 429                                //Codigo Http de retorno de estado
            //            },
            //        }
            //    };
            //    options.EnableEndpointRateLimiting = true;                  // Ativa cota de limite para endpoint customizado
            //    options.EnableRegexRuleMatching = true;                     // Habilita Regex
            //});

            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("localhost:6379")); //Redis IP
            services.AddRedisRateLimiting();
            // services.AddDistributedRateLimiting<RedisProcessingStrategy>();

            // services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();



            // inject counter and rules distributed cache stores
            //  services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            // services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();


            #endregion
            // Add framework services.
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Estudo RateLimit v1"));
            }

            app.UseHttpsRedirection();

            app.UseIpRateLimiting();    // Ativa o uso do Middleware de RateLimit

            app.UseRouting();
          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
