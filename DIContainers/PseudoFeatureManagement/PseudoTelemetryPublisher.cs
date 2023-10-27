namespace Experiment.PseudoFeatureManagement
{
    public class PseudoTelemetryPublisher : ITelemetryPublisher
    {
        public void Publish()
        {
            Console.WriteLine("Publish some telemetry.");
        }

        public void Hello()
        {
            Console.WriteLine("Greeting World.");
        }
    }
}
