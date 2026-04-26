using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MediTap.Api.Models.Converters
{
    public class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
    {

        public PhoneNumberConverter() : base(
            v => v.Number, // Convert PhoneNumber to string for storage
            v => new PhoneNumber(v)) // Convert string back to PhoneNumber when reading from the database
        {
        }

    }
}
