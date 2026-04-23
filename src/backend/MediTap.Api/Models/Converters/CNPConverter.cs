using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MediTap.Api.Models.Converters
{
    public class CNPConverter : ValueConverter<CNP, string>
    {

        public CNPConverter()

            : base(

                 v => v.CodNumericPersonal, // Converts CNP to string for storage
                 v => new CNP(v) // Converts string back to CNP when reading from the database

                 )

        {

        }
    }
}
