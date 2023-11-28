using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using System.Reflection;

namespace DIContainers
{
    public static class Demo
    {
        public static void DemoMEDI()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMyLogger, MyLogger>();

            services.AddSingleton(sp => new ComponentA()
            {
                Logger = sp.GetRequiredService<IMyLogger>()
            });

            services.AddSingleton(sp => new ComponentB()
            {
                Logger = sp.GetRequiredService<IMyLogger>()
            });

            services.AddTransient<IStrategy, StrategyA>();

            services.AddTransient<IStrategy, StrategyB>();

            services.AddSingleton<IApplication>(sp => new DemoAppWithMEDI(
                sp.GetRequiredService<IEnumerable<IStrategy>>())
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
        }

        public static void DemoAutofac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MyLogger>().As<IMyLogger>().SingleInstance();

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
        }

        public static async void UseFeatureManagementWithAutofac()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(config).As<IConfiguration>();

            builder.RegisterType<ConfigurationFeatureDefinitionProvider>().As<IFeatureDefinitionProvider>();

            builder.RegisterInstance(new FeatureManagementOptions()).AsSelf().SingleInstance();

            builder.Register(c => new FeatureManager(
                c.Resolve<IFeatureDefinitionProvider>(),
                c.Resolve<FeatureManagementOptions>())
            {
                FeatureFilters = c.Resolve<IEnumerable<IFeatureFilterMetadata>>()
            }).As<IFeatureManager>().SingleInstance();

            builder.RegisterInstance(Options.Create(new TargetingEvaluationOptions()))
                .As<IOptions<TargetingEvaluationOptions>>().SingleInstance();

            builder.RegisterInstance(LoggerFactory.Create(builder => builder.AddConsole()))
                .As<ILoggerFactory>().SingleInstance();

            var targetingContextAccessor = new OnDemandTargetingContextAccessor();

            builder.RegisterInstance(targetingContextAccessor).As<ITargetingContextAccessor>().SingleInstance();

            builder.RegisterType<TargetingFilter>().As<IFeatureFilterMetadata>().SingleInstance();

            var container = builder.Build();

            IFeatureManager featureManager = container.Resolve<IFeatureManager>();

            var users = new List<string>()
            {
                "Jeff",
                "Sam"
            };

            const string feature = "Beta";

            foreach (var user in users)
            {
                targetingContextAccessor.Current = new TargetingContext
                {
                    UserId = user
                };

                Console.WriteLine($"{feature} is {(await featureManager.IsEnabledAsync(feature) ? "enabled" : "disabled")} for {user}.");
            }
        }
    }
}
