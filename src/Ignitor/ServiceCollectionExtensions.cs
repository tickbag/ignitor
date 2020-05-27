using System.Diagnostics.CodeAnalysis;
using Ignitor.State;
using Ignitor.StateMonitor;
using Ignitor.Transient;
using Microsoft.Extensions.DependencyInjection;

namespace Ignitor
{
    /// <summary>
    /// Extension to the Service Collection for adding necessary Ignitor services
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the ignitor framework to this application.
        /// Registers all required services for Ignitor to work.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddIgnitor(this IServiceCollection services)
        {
            // Register the state services
            services.AddSingleton<IState, State.State>();
            services.AddSingleton(typeof(IState<>), typeof(State<>));

            services.AddTransient(typeof(IScopedState<,>), typeof(ScopedState<,>));

            services.AddTransient<IScopedStateFactory, ScopedStateFactory>();

            // Register the Concurrent Store (Dictionary)
            services.AddTransient(typeof(IIgnitorStore<,>), typeof(IgnitorStore<,>));

            // State monitoring system
            services.AddTransient(typeof(IStateMonitor<,>), typeof(StateMonitor<,>));

            return services;
        }
    }
}
