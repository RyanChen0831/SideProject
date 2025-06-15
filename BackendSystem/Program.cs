
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
            // �`�J Redis
            var connectionStringForRedis = builder.Configuration.GetConnectionString("Redis");
            var configuration = ConfigurationOptions.Parse(connectionStringForRedis, true);
            configuration.ResolveDns = true;
            configuration.AbortOnConnectFail = false;

            builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(configuration);
            });
            //�`�J CORS
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
            // �`�J Dapper �A��
            var connectionString = builder.Configuration.GetConnectionString("DesertShopDbContext");
            builder.Services.AddScoped<IDbConnection>(conn => new SqlConnection(connectionString));
            // �`�J AutoMapper �A��
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //�`�JService
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
            //�`�JCookie
            // �`�J Cookie �{�ҪA��
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "UserCookie";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.Cookie.SameSite = SameSiteMode.None; // �󯸶ǻ� Cookie
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