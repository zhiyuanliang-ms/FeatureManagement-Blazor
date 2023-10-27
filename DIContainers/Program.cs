using Autofac;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Experiment.PseudoFeatureManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ninject;
using Unity;

namespace Experiment
{
    class Program
    {
        static void MicrosoftDI()
        {
            Console.WriteLine("========== Using Microsoft.Extensions.DependencyInjection ==========");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();

            services.AddSingleton(config);

            services.AddLogging();

            services.AddSingleton<IFeatureFlagProvider, PseudoFeatureFlagProvider>();

            services.AddSingleton<ITelemetryPublisher, PseudoTelemetryPublisher>();

            services.AddSingleton<ITelemetryPublisher, DummyTelemetryPublisher>();

            services.AddSingleton<IFeatureManager>(sp => new PseudoFeatureManager(
                sp.GetRequiredService<IFeatureFlagProvider>(),
                sp.GetRequiredService<IOptions<PseudoOptions>>().Value)
            {
                Logger = sp.GetService<ILoggerFactory>()?.CreateLogger<PseudoFeatureManager>(),
                TelemetryPublishers = sp.GetService<IEnumerable<ITelemetryPublisher>>()
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFeatureManager featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

            featureManager.Log();

            featureManager.PublishTelemetry();

            featureManager.GetFeatureFlag("Beta");
        }

        static void AutofacDI()
        {
            Console.WriteLine("========== Using Autofac ==========");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(config).As<IConfiguration>();

            builder.RegisterType<PseudoFeatureFlagProvider>().As<IFeatureFlagProvider>();

            builder.RegisterInstance(new PseudoOptions()).AsSelf();

            builder.RegisterInstance(new LoggerFactory()).As<ILoggerFactory>();

            builder.RegisterType<PseudoTelemetryPublisher>().As<ITelemetryPublisher>();

            builder.RegisterType<DummyTelemetryPublisher>().As<ITelemetryPublisher>();

            builder.Register(c => new PseudoFeatureManager(
                c.Resolve<IFeatureFlagProvider>(),
                c.Resolve<PseudoOptions>())
            {
                Logger = c.Resolve<ILoggerFactory>().CreateLogger<PseudoFeatureManager>(),
                TelemetryPublishers = c.Resolve<IEnumerable<ITelemetryPublisher>>()
            }).As<IFeatureManager>();

            var container = builder.Build();

            IFeatureManager featureManager = container.Resolve<IFeatureManager>();

            featureManager.Log();

            featureManager.PublishTelemetry();

            featureManager.GetFeatureFlag("Beta");
        }

        static void UnityDI()
        {
            Console.WriteLine("========== Using Unity ==========");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var container = new UnityContainer();

            container.RegisterInstance(config);

            container.RegisterType<IFeatureFlagProvider, PseudoFeatureFlagProvider>();

            container.RegisterInstance(new PseudoOptions());

            container.RegisterInstance<ILoggerFactory>(new LoggerFactory());

            container.RegisterType<ITelemetryPublisher, PseudoTelemetryPublisher>("Publisher1");

            container.RegisterType<ITelemetryPublisher, DummyTelemetryPublisher>("Publisher2");

            container.RegisterFactory<IFeatureManager>(f => new PseudoFeatureManager(
                f.Resolve<IFeatureFlagProvider>(),
                f.Resolve<PseudoOptions>())
            {
                Logger = f.Resolve<ILoggerFactory>().CreateLogger<PseudoFeatureManager>(),
                TelemetryPublishers = f.ResolveAll<ITelemetryPublisher>()
            });

            IFeatureManager featureManager = container.Resolve<IFeatureManager>();

            featureManager.Log();

            featureManager.PublishTelemetry();

            featureManager.GetFeatureFlag("Beta");
        }

        static void NinjectDI()
        {
            Console.WriteLine("========== Using Ninject ==========");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            IFeatureManager featureManager;

            var kernel = new StandardKernel();

            kernel.Bind<IConfiguration>().ToConstant(config);

            kernel.Bind<IFeatureFlagProvider>().To<PseudoFeatureFlagProvider>();

            kernel.Bind<PseudoOptions>().ToConstant(new PseudoOptions());

            kernel.Bind<ILoggerFactory>().ToConstant(new LoggerFactory());

            kernel.Bind<ITelemetryPublisher>().To<PseudoTelemetryPublisher>();

            kernel.Bind<ITelemetryPublisher>().To<DummyTelemetryPublisher>();

            kernel.Bind<IFeatureManager>().ToMethod(c => new PseudoFeatureManager(
                c.Kernel.Get<IFeatureFlagProvider>(),
                c.Kernel.Get<PseudoOptions>())
            {
                Logger = c.Kernel.Get<ILoggerFactory>().CreateLogger<PseudoFeatureManager>(),
                TelemetryPublishers = c.Kernel.Get<IEnumerable<ITelemetryPublisher>>()
            });

            featureManager = kernel.Get<IFeatureManager>();

            featureManager.Log();

            featureManager.PublishTelemetry();

            featureManager.GetFeatureFlag("Beta");
        }

        static void CastleWindsorDI()
        {
            Console.WriteLine("========== Using Castle Windsor ==========");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var container = new WindsorContainer();

            container.Register(Component.For<IConfiguration>().Instance(config));

            container.Register(Component.For<IFeatureFlagProvider>()
                .ImplementedBy<PseudoFeatureFlagProvider>());

            container.Register(Component.For<PseudoOptions>()
                .Instance(new PseudoOptions()));

            container.Register(Component.For<ILoggerFactory>()
                .Instance(new LoggerFactory()));

            container.Register(Component.For<ITelemetryPublisher>()
                .ImplementedBy<PseudoTelemetryPublisher>().Named("Publisher1"));

            container.Register(Component.For<ITelemetryPublisher>()
                .ImplementedBy<DummyTelemetryPublisher>().Named("Publisher2"));

            container.Register(Component.For<IFeatureManager>().UsingFactoryMethod(kernel => new PseudoFeatureManager(
                kernel.Resolve<IFeatureFlagProvider>(),
                kernel.Resolve<PseudoOptions>())
            {
                Logger = kernel.Resolve<ILoggerFactory>().CreateLogger<PseudoFeatureManager>(),
                TelemetryPublishers = kernel.ResolveAll<ITelemetryPublisher>()
            }));

            IFeatureManager featureManager = container.Resolve<IFeatureManager>();

            featureManager.Log();

            featureManager.PublishTelemetry();

            featureManager.GetFeatureFlag("Beta");
        }

        static void Main()
        {
            MicrosoftDI();

            AutofacDI();

            UnityDI();

            NinjectDI();

            CastleWindsorDI();
        }
    }
}