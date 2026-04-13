using MediTap.Api.Exceptions;

namespace MediTap.Api.Models
{
    public class Email
    {
        public string EmailAddress { get; set; }

        public bool IsValid { get; set; }

        public Email(string emailAddress)
        {
            IsValid = checkEmailValidity(emailAddress);
            if(IsValid)
            {
                EmailAddress = emailAddress;
            }
            else
            {
                throw new InvalidEmailException("Invalid email provided: " + emailAddress);

            }

        }

        // TODO 
        // Add methods to check if email is valid or not
    }
}
