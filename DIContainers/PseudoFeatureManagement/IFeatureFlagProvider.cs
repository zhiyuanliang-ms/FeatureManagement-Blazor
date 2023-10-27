namespace Experiment.PseudoFeatureManagement
{
    public interface IFeatureFlagProvider
    {
        public bool GetFeatureFlag(string name);
    }
}
