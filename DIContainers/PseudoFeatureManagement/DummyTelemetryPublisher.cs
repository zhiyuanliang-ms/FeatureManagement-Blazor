namespace Experiment.PseudoFeatureManagement
{
    public class DummyTelemetryPublisher : ITelemetryPublisher, IHello
    {
        public void Publish()
        {
            Console.WriteLine("Publish some dummy telemetry.");
        }

        public void Greeting()
        {
            Console.WriteLine("Hello World.");
        }
    }
}
