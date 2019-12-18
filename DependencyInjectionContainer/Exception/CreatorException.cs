namespace DependencyInjectionContainer.Exception
{
    public class CreatorException : System.Exception
    {
        public CreatorException(string message) : base(message)
        {
        }

        public CreatorException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
