using Microsoft.Extensions.Configuration;

namespace Experiment.PseudoFeatureManagement
{
    public class PseudoFeatureFlagProvider : IFeatureFlagProvider
    {
        private readonly IConfiguration _configuration;

        public PseudoFeatureFlagProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public bool GetFeatureFlag(string name)
        {
            IConfigurationSection featureManagementConfigurationSection = _configuration.GetSection("FeatureManagement");

            IEnumerable<IConfigurationSection> featureFlagSection = featureManagementConfigurationSection.GetChildren();

            IConfigurationSection? featureFlag = featureFlagSection.FirstOrDefault(section => section.Key.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (featureFlag == null)
            {
                return false;
            }

            string? val = featureFlag.Value;

            bool.TryParse(val, out bool result);

            return result;
        }
    }
}
