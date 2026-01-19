namespace Nyx.Host
{
    public static class NyxBootstrap
    {
        public static ServiceProvider CreateDefault()
        {
            ServiceCollection services = new ServiceCollection();




            return services.BuildServiceProvider();
        }
    }
}
