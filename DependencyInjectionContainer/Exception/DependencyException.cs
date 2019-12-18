namespace DependencyInjectionContainer.Exception
{
    public class DependencyException : System.Exception
    {
        public DependencyException(string message) : base(message)
        {
        }

        public DependencyException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
