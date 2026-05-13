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

            // Find all classes that implement interfaces ending in "Service"
            //var serviceTypes = assembly.GetTypes()
            //    .Where(t => t.Name.EndsWith("Service") && t.IsClass && !t.IsAbstract)
            //    .ToList();


            // Register each service and its interface
            //foreach (var serviceType in serviceTypes)
            //{
            //    // Find the corresponding interface
            //    var interfaces = serviceType.GetInterfaces()
            //        .Where(i => i.Name == "I" + serviceType.Name) // Matching convention: I<ServiceName> interface
            //        .ToList();

            //    foreach (var @interface in interfaces)
            //    {
            //        // Register with scoped lifetime
            //        services.AddScoped(@interface, serviceType);
            //    }
            //}


            // Get all the types in the current assembly
            var types = assembly.GetTypes();

            // Find classes that implement interfaces and are not abstract
            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract))
            {
                // Get all interfaces implemented by the class
                var interfaces = type.GetInterfaces();

                // Register each service with its corresponding interface(s)
                foreach (var @interface in interfaces)
                {
                    // Register the service for each matching interface
                    services.AddScoped(@interface, type);
                }
            }


            return services;
        }
    }
}
