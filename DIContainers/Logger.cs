namespace DIContainers
{
    public interface ILogger
    {
        public void Info(string message);
    }

    public class Logger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }
    }
}
