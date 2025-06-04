using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FullControl.Validators
{
    public class EmailValidator : ValidationRule
    {
        public EmailValidator() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? stringValue = value as string;

            if (!IsEmailValid(stringValue))
            {
                
                return new ValidationResult(false, "Deve ser um email.");
            }

            return ValidationResult.ValidResult;
        }

        public bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
