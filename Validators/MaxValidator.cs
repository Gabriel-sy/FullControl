using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FullControl.Validators
{
    public class MaxValidator : BaseCustomValidator
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string stringValue = value as string;

            if (stringValue.Length > Prop)
            {
                return new ValidationResult(false, "Deve ser menor que " + Prop);
            }

            return ValidationResult.ValidResult;
        }
    }
}
