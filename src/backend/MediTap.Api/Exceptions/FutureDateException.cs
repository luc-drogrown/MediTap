namespace MediTap.Api.Exceptions
{
    public class FutureDateException : Exception
    {
        public FutureDateException() : base("Selected date is in the future.") { }
    }
}
