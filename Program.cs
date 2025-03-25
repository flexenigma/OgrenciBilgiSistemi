using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace OgrenciBilgiSistemi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // MySQL veritabanı bağlantısı
            var connectionString = "server=localhost;port=3306;database=OgrenciBilgiSistemi;user=root;password=Ss123123;CharSet=utf8mb4;";
            builder.Services.AddDbContext<VeriTabaniContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysqlOptions =>
                    {
                        // Bağlantı koptuğunda yeniden deneme mekanizması
                        mysqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                        
                        // Veritabanı komutları için zaman aşımı süresi
                        mysqlOptions.CommandTimeout(60);
                    }
                );
                
                // MySQL bağlantısı için detaylı hata mesajları
                options.EnableDetailedErrors(true);
                // SQL sorgularını log'a kaydetme
                options.EnableSensitiveDataLogging(true);
            });

            // MVC yapılandırması
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                });

            // HTTP ve Response için karakter kodlamasını ayarlama
            builder.Services.AddWebEncoders(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            // FluentValidation yapılandırması
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Authentication yapılandırması
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Kullanici/Login";
                    options.AccessDeniedPath = "/Kullanici/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            // HttpContext erişimi için
            builder.Services.AddHttpContextAccessor();

            // Custom servislerin eklenmesi
            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<IRaporService, RaporService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Veritabanını otomatik oluştur
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<VeriTabaniContext>();
                context.Database.EnsureCreated();
            }

            app.Run();
        }

        // Şifre hashleme yardımcı metodu
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
                
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // UTF8 encoding kullanarak şifreyi byte dizisine dönüştür
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                // Byte dizisini hex formatına dönüştür
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                
                // Tüm harfleri büyük harfe çevir - karşılaştırmalarda tutarlılık için
                return builder.ToString().ToUpper();
            }
        }
    }
} 