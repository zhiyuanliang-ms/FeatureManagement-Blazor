namespace DIContainers
{
    public interface IComponent
    {
    }

    public abstract class Component : IComponent
    {
        public IMyLogger Logger { get; set; }

        public abstract void DoSomething();
    }

    public class ComponentA : Component 
    { 
        public override void DoSomething()
        {
            Console.WriteLine("ComponentA is working.");
        }
    }

    public class ComponentB : Component
    {
        public override void DoSomething()
        {
            Console.WriteLine("ComponentB is working.");
        }
    }
}
