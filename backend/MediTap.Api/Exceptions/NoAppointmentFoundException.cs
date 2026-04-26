namespace MediTap.Api.Exceptions
{
    public class NoAppointmentFoundException : Exception
    {
        public NoAppointmentFoundException() : base("No appointment found for the specified patient and medic.") { }
    }
}
