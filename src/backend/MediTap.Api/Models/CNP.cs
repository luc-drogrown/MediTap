using MediTap.Api.Exceptions;

namespace MediTap.Api.Models
{
    /// <summary>
    /// This is a helper function
    /// It is used to store CNP's and to make sure they are valid.
    /// </summary>
    public class CNP
    {
        public string CodNumericPersonal { get; set; }
        public bool isValid { get; set; }

        public CNP( string codNumericPersonal)
        {
            this.isValid = checkCNPValidity(codNumericPersonal);
            if(isValid)
            {
                this.CodNumericPersonal = codNumericPersonal;
            }
            else
            {
                throw new InvalidCNPException("Invalid CNP provided: " + codNumericPersonal);
            }

        }

        // TODO
        // Add methods to check if a CNP is valid or not


    }
}
