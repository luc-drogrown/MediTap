namespace MediTap.Api.Exceptions
{
    public class UnameAlreadyExistsException : Exception
    {
        public UnameAlreadyExistsException() : base ("Username already exists") { }
    }
}
