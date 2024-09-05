using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data;
using TaskPro.Data.Entidades;
using TaskPro.Services;

namespace TaskPro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DataContext>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSQL"));
            });

            builder.Services.AddTransient<SeedDb>();
            builder.Services.AddScoped<IServicioUsuario, ServicioUsuario>();
            builder.Services.AddIdentity<Usuario, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<DataContext>();

            var app = builder.Build();

            SeedData(app);
            void SeedData(WebApplication app)
            {
                IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>()!;
                using (IServiceScope scope = scopedFactory!.CreateScope())
                {
                    SeedDb service = scope.ServiceProvider.GetService<SeedDb>()!;
                    service.SeedAsync().Wait();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.Run();
        }
    }
}