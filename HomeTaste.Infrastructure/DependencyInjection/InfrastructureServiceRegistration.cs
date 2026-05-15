using AuthGuard.Infrastructure.Services;
using CloudinaryDotNet;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Application.Interfaces.Realtime;
using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Settings;
using HomeTaste.Infrastructure.Hubs;
using HomeTaste.Infrastructure.Identity;
using HomeTaste.Infrastructure.Identity.Entity;
using HomeTaste.Infrastructure.Payments;
using HomeTaste.Infrastructure.Persistence;
using HomeTaste.Infrastructure.Persistence.Repositories;
using HomeTaste.Infrastructure.Services;
using HomeTaste.Infrastructure.Services.FileStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeTaste.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection("JwtSettings"));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddIdentityCore<IdentityApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();


            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IUserManager, IdentityUserManager>();
            services.AddScoped<IRoleManager, RoleManager>();
            services.AddScoped<ISignInManager, IdentitySignInManager>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IUserContextService, UserContextService>();

            services.AddScoped<IStripeService, StripeService>();

            // Payment processor strategy pattern
            services.AddScoped<IPaymentProcessor, StripePaymentProcessor>();
            services.AddScoped<IPaymentProcessor, BKashManualPaymentProcessor>();
            services.AddScoped<IPaymentProcessor, BKashCheckoutPaymentProcessor>();
            services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();

            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();
            var cloudinarySettings = config.GetSection("CloudinarySettings");

            var account = new Account(
                cloudinarySettings["CloudName"],
                cloudinarySettings["ApiKey"],
                cloudinarySettings["ApiSecret"]
            );

            services.AddSingleton(new Cloudinary(account));

            services.AddScoped<IFileStorage, CloudinaryFileStorage>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            services.AddSignalR();
            services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

            return services;
        }
    }
}
