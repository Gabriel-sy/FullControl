using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            if (!stringValue.Contains("@"))
            {
                return new ValidationResult(false, "Deve ser um email.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
