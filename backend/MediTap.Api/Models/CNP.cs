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

        public CNP( string codNumericPersonal)
        {
            bool isValid = checkCNPValidity(codNumericPersonal);
            if(isValid)
            {
                this.CodNumericPersonal = codNumericPersonal;
            }
            else
            {
                throw new InvalidCNPException("Invalid CNP provided: " + codNumericPersonal);
            }

        }

        // Method to check if a CNP is valid or not
        private bool checkCNPValidity(string cnp) {
            
            if(cnp.Length != 13) { return false; }

            var CNP_Constant = "279146358279";
            double sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += Char.GetNumericValue(cnp[i]) * Char.GetNumericValue(CNP_Constant[i]);
            }
            int foundControlDigit;

            if ((sum % 11) < 10)
            {
                foundControlDigit = (int)(sum % 11);
            }
            else { 
                foundControlDigit = 1;
            }

            int controlDigit = (int)Char.GetNumericValue(cnp[12]);


            return foundControlDigit == controlDigit; 
        
        }

        // TODO -> override object.Equals so that two CNP's are equal if their string property is the same



    }
}
