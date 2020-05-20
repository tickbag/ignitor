using Ignitor.Cloning;
using Ignitor.Transient;
using Microsoft.Extensions.DependencyInjection;
using static Ignitor.State.StateHelpers;

namespace Ignitor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIgnitor(this IServiceCollection services)
        {
            // Register Cloner
            services.AddSingleton(typeof(ICloner<>), typeof(Cloner<>));

            // Register the Concurrent Store (Dictionary)
            services.AddSingleton(typeof(IIgnitorStore<,,>), typeof(IgnitorStore<,,>));

            // Register the state services
            services.AddTransient<IState, State.State>();
            services.AddTransient(typeof(IState<,>), typeof(StateHelper<,>));
            services.AddTransient(typeof(IState<,,>), typeof(StateHelper<,,>));

            return services;
        }
    }
}
