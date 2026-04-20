using MediTap.Api.Exceptions;
using PhoneNumbers;
namespace MediTap.Api.Models
{
    /// <summary>
    /// This is a helper function
    /// It is used to store phone numbers and to make sure they are valid.
    /// It uses a Google library called libphonenumber to validate the phone numbers.
    /// The library supports phone numbers from all over the world, 
    /// but for now we will assume that only Romanian phone numbers are allowed.
    /// </summary>
    public class PhoneNumber
    {
        public string Number { get; set; }
        public bool isValid { get; set; }

        public static PhoneNumbers.PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();



        public PhoneNumber(string Number)
        {
            try
            {
                var parsedNumber = phoneUtil.Parse(Number, "RO"); // Assumes only RO numbers allowed
                this.isValid = phoneUtil.IsValidNumber(parsedNumber);
                if (isValid)
                {
                    this.Number = Number;
                }
                else
                {
                    throw new InvalidPhoneNumberException("Invalid phone number provided: " + Number);
                }
            }
            catch (PhoneNumbers.NumberParseException)
            {

                throw new InvalidPhoneNumberException("Unparsable phone number provided: " + Number);

            }
        }
    }
}
