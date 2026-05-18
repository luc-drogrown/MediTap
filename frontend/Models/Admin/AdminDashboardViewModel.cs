namespace MediTap.Front.Models.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalPatients { get; set; }

        public int TotalMedics { get; set; }

        public int PendingPasswordResetRequests { get; set; }

        public int InactiveAccounts { get; set; }
    }
}