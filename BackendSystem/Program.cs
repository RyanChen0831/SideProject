
using Amazon.S3;
using BackendSystem.Respository.Implement;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.Implement;
using BackendSystem.Service.Interface;
using BackendSystem.Service.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Data;


namespace BackendSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();
            builder.Services.AddAWSService<IAmazonS3>();
            // 注入 Redis
            var connectionStringForRedis = builder.Configuration.GetConnectionString("Redis");
            var configuration = ConfigurationOptions.Parse(connectionStringForRedis, true);
            configuration.ResolveDns = true;
            configuration.AbortOnConnectFail = false;

            builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(configuration);
            });
            //注入 CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("https://localhost:5174", "https://localhost:5173")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
                }
                );
            });
            // 注入 Dapper 服務
            var connectionString = builder.Configuration.GetConnectionString("DesertShopDbContext");
            builder.Services.AddScoped<IDbConnection>(conn => new SqlConnection(connectionString));
            // 注入 AutoMapper 服務
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //注入Service
            builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
            builder.Services.AddScoped<IShoppingCartRespository, ShoppingCartRespository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductRespository, ProductRespository>();
            builder.Services.AddScoped<IOrderRespository, OrderRespository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IMemberRespository, MemberRespository>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<ITokenService, TokenManager>();
            builder.Services.AddScoped<IS3Service, S3Service>();
            builder.Services.AddHttpContextAccessor();
            //注入Cookie
            // 注入 Cookie 認證服務
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "UserCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.Cookie.SameSite = SameSiteMode.None; // 跨站傳遞 Cookie
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }
                    return Task.CompletedTask;
                };
            });


            builder.Services.AddMvc(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("MyAllowSpecificOrigins");
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapControllers();

            app.Run();
        }
    }
}