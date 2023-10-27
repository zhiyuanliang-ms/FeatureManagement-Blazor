namespace Experiment.PseudoFeatureManagement
{
    public interface IFeatureManager
    {
        public void Log();

        public void PublishTelemetry();

        public void GetFeatureFlag(string name);
    }
}
