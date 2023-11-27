using Autofac.Features.Indexed;
using Microsoft.FeatureManagement;

namespace DIContainers
{
    public interface IApplication
    {
        public ComponentA A { get; }

        public ComponentB B { get; }

        public void RunStrategy(string strategyName);

        public Task WorkAsync();
    }

    public class DemoAppWithAutofac : IApplication
    {
        private readonly IIndex<string, IStrategy> _strategies;

        private readonly IFeatureManager _featureManager;

        public ComponentA A { get; init; }

        public ComponentB B { get; init; }

        public DemoAppWithAutofac(IIndex<string, IStrategy> strategies, IFeatureManager featureManager)
        {
            _strategies = strategies;
            _featureManager = featureManager;
        }

        public void RunStrategy(string strategyName)
        {
            if (_strategies.TryGetValue(strategyName, out IStrategy strategy))
            {
                strategy.Run();
            }
        }

        public async Task WorkAsync()
        {
            if (await _featureManager.IsEnabledAsync("Beta"))
            {
                Console.WriteLine("Beta feature flag is on.");
            }
        }
    }

    public class DemoAppWithMEDI : IApplication
    {
        private readonly IEnumerable<IStrategy> _strategies;

        private readonly IFeatureManager _featureManager;

        public ComponentA A { get; init; }

        public ComponentB B { get; init; }

        public DemoAppWithMEDI(IEnumerable<IStrategy> strategies, IFeatureManager featureManager)
        {
            _strategies = strategies;
            _featureManager = featureManager;
        }

        public void RunStrategy(string strategyName)
        {
            _strategies.FirstOrDefault(s => s.Name == strategyName)?.Run();
        }

        public async Task WorkAsync()
        {
            if (await _featureManager.IsEnabledAsync("Beta"))
            {
                Console.WriteLine("App is working and the Beta feature flag is on.");
            }
        }
    }
}
