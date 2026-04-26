using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MediTap.Api.Models.Converters
{
    /// <summary>
    /// This classed is used to convert the Email type
    /// into a string in order for it to be stored in the DB
    /// more info here: https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#bulk-configuring-a-value-converter
    /// </summary>
    public class EmailConverter: ValueConverter<Email, string>
    {

        public EmailConverter()
        
            :base (
                 
                 v => v.EmailAddress, // Converts Email to string for storage
                 v => new Email(v) // Converts string back to Email when reading from the database
                 
                 )
        
        {
            
        }

    }
}
