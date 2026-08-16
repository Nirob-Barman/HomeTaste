//using HomeTaste.Application.Interfaces.Auth;
//using HomeTaste.Application.Interfaces.MealManagement;
//using HomeTaste.Application.Interfaces.TaskManagement;
//using HomeTaste.Application.Interfaces.Test;
//using HomeTaste.Application.Interfaces.Measurements;
//using HomeTaste.Application.Services;
//using HomeTaste.Application.Services.MealManagement;
//using HomeTaste.Application.Services.TaskManagement;
//using HomeTaste.Application.Services.Test;
//using HomeTaste.Application.Services.Measurements;
using FluentValidation;
using HomeTaste.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HomeTaste.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IUnitService, UnitService>();
            //services.AddScoped<IMealCategoryService, MealCategoryService>();
            //services.AddScoped<IIngredientService, IngredientService>();
            //services.AddScoped<IMealService, MealService>();
            //services.AddScoped<IMealIngredientService, MealIngredientService>();
            //services.AddScoped<ITaskService, TaskService>();
            //services.AddScoped<ITransactionTestService, TransactionTestService>();


            // Get the current assembly
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(assembly);

            // Get all the types in the current assembly
            var types = assembly.GetTypes();

            // Find classes that implement interfaces and are not abstract
            // (skip open generic type definitions like ValidationBehavior<,> - those are
            // registered explicitly above via AddOpenBehavior/AddValidatorsFromAssembly)
            //
            // Only register a type against an interface following the I<ClassName> naming
            // convention (e.g. UnitService -> IUnitService). A blanket "register every
            // interface a class implements" swept up MediatR Commands/Queries (via
            // IRequest<T>/IBaseRequest) and Exception types (via ISerializable) as if they
            // were injectable services - their constructors take plain values (Guid, string,
            // DTOs), which the container can't resolve, and ASP.NET Core validates this
            // eagerly at startup when ValidateOnBuild is on (the Development-environment
            // default), crashing WebApplicationBuilder.Build().
            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition))
            {
                var interfaces = type.GetInterfaces().Where(i => i.Name == "I" + type.Name);

                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, type);
                }
            }


            return services;
        }
    }
}
