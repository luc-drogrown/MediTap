namespace MediTap.Api.Exceptions
{
    public class CNPAlreadyExistsException : Exception
    {
        public CNPAlreadyExistsException() : base ("CNP already exists")
        {
            
        }
    }
}
