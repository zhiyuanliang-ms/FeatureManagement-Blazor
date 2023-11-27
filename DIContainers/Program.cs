using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Reflection;


namespace DIContainers
{
    class Program
    {
        static void DemoMEDI()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();

            services.AddSingleton(config);

            services.AddFeatureManagement();

            services.AddSingleton<ILogger, Logger>();

            services.AddSingleton(sp => new ComponentA()
            {
                Logger = sp.GetRequiredService<ILogger>()
            });

            services.AddSingleton(sp => new ComponentB()
            {
                Logger = sp.GetRequiredService<ILogger>()
            });

            services.AddTransient<IStrategy, StrategyA>();

            services.AddTransient<IStrategy, StrategyB>();

            services.AddSingleton<IApplication>(sp => new DemoAppWithMEDI(
                sp.GetRequiredService<IEnumerable<IStrategy>>(),
                sp.GetRequiredService<IFeatureManager>())
            {
                A = sp.GetRequiredService<ComponentA>(),
                B = sp.GetRequiredService<ComponentB>()
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IApplication app = serviceProvider.GetRequiredService<IApplication>();

            app.RunStrategy("StrategyA");

            app.RunStrategy("StrategyA");

            app.A.DoSomething();

            app.A.Logger.Info("Component A of the App with MEDI is doing something.");

            app.B.DoSomething();

            app.B.Logger.Info("Component B of the App with MEDI is doing something.");

            app.WorkAsync();
        }

        static void DemoAutofac()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var builder = new ContainerBuilder();

            builder.RegisterType<Logger>()
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterInstance(config).As<IConfiguration>();

            builder.RegisterType<ConfigurationFeatureDefinitionProvider>().As<IFeatureDefinitionProvider>();

            builder.RegisterInstance(new FeatureManagementOptions()).AsSelf();

            builder.RegisterType<FeatureManager>().As<IFeatureManager>();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Component)))
                .Where(type => type.IsSubclassOf(typeof(Component)))
                .PropertiesAutowired()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<StrategyA>().Keyed<IStrategy>("StrategyA");

            builder.RegisterType<StrategyB>().Keyed<IStrategy>("StrategyB");

            builder.RegisterType<DemoAppWithAutofac>()
                .PropertiesAutowired()
                .As<IApplication>()
                .SingleInstance();

            var container = builder.Build();

            var app = container.Resolve<IApplication>();

            app.RunStrategy("StrategyA");

            app.RunStrategy("StrategyA");

            app.A.DoSomething();

            app.A.Logger.Info("Component A of the App with Autofac is doing something.");

            app.B.DoSomething();

            app.B.Logger.Info("Component B of the App with Autofac is doing something.");

            app.WorkAsync();
        }

        static void Main()
        {
            //DemoMEDI();

            DemoAutofac();
        }
    }
}