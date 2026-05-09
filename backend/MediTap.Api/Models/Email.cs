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

        private bool checkEmailValidity(string email)
        {
            if(email == null || !email.Contains("@"))
            {
                return false;
            }

            var parts = email.Split('@');

            if (parts.Length == 2)
            {
                if (!parts[1].Contains("."))
                {
                    return false;
                }

                if (parts[0].Length < 1)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }


        }

    }
}
