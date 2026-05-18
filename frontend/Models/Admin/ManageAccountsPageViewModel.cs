namespace MediTap.Front.Models.Admin
{
    public class ManageAccountsPageViewModel
    {
        public List<ManageAccountViewModel> Accounts { get; set; } = new();

        public List<PasswordResetRequestViewModel> PasswordResetRequests { get; set; } = new();
    }
}
