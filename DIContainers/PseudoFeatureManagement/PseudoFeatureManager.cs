using Microsoft.Extensions.Logging;

namespace Experiment.PseudoFeatureManagement
{
    public class PseudoFeatureManager : IFeatureManager
    {
        private readonly IFeatureFlagProvider _featureFlagProvider;
        private readonly PseudoOptions _options;

        public PseudoFeatureManager(
            IFeatureFlagProvider featureDefinitionProvider,
            PseudoOptions options)
        {
            _featureFlagProvider = featureDefinitionProvider ?? throw new ArgumentNullException(nameof(featureDefinitionProvider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ILogger? Logger { get; init; }

        public IEnumerable<ITelemetryPublisher>? TelemetryPublishers { get; init; }

        public void Log()
        {
            if (Logger == null)
            {
                Console.WriteLine("Logger is null.");
            }
            else
            {

                Logger.LogInformation("Some information.");

                Console.WriteLine("Log something.");
            }
        }

        public void PublishTelemetry()
        {
            if (TelemetryPublishers == null)
            {
                Console.WriteLine("TelemetryPublishers is null.");
            }
            else
            {
                foreach (ITelemetryPublisher telemetryPublisher in TelemetryPublishers)
                {
                    telemetryPublisher.Publish();

                    if (telemetryPublisher is IHello hello)
                    {
                        hello.Greeting();
                    }
                }
            }
        }

        public void GetFeatureFlag(string name)
        {
            if (_options.AnOption)
            {
                Console.WriteLine("Option is on.");
                Console.WriteLine($"{name} is {_featureFlagProvider.GetFeatureFlag(name)}");
            }
            else
            {
                Console.WriteLine("Option is off.");
            }
        }
    }
}
