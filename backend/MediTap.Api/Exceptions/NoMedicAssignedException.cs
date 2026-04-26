namespace MediTap.Api.Exceptions
{
    public class NoMedicAssignedException : Exception
    {
        public NoMedicAssignedException() : base("No medic assigned to this patient.")
        {
        }
    }
}
