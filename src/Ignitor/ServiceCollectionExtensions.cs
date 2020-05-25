using Ignitor.State;
using Ignitor.StateMonitor;
using Ignitor.Transient;
using Microsoft.Extensions.DependencyInjection;

namespace Ignitor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIgnitor(this IServiceCollection services)
        {
            // Register the state services
            services.AddSingleton<IState, State.State>();
            services.AddSingleton(typeof(IState<>), typeof(State<>));

            services.AddTransient(typeof(IScopedState<,>), typeof(ScopedState<,>));

            // Register the Concurrent Store (Dictionary)
            services.AddTransient(typeof(IIgnitorStore<,>), typeof(IgnitorStore<,>));

            // State monitoring system
            services.AddTransient(typeof(IStateMonitor<,>), typeof(StateMonitor<,>));

            return services;
        }
    }
}
