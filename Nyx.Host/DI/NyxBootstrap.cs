using Nyx.Core.Diagnostics.Logging;
using Nyx.Host.Diagnostics;

namespace Nyx.Host
{
    public static class NyxBootstrap
    {
        public static ServiceProvider CreateDefault()
        {
            ServiceCollection services = new ServiceCollection();

            // TODO

            services.AddSingleton<INyxLogger, NyxLogger>();
            services.AddSingleton(typeof(INyxContextLogger<>), typeof(NyxContextLogger<>));

            return services.BuildServiceProvider();
        }
    }
}
