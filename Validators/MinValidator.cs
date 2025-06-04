using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FullControl.Validators
{
    public class MinValidator : ValidationRule
    {
        public int Prop { get; set; } = 3;
        public MinValidator() { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valor = value as string;

            if (valor.Length < Prop)
            {
                return new ValidationResult(false, "Deve ser maior que " + Prop);
            }
            return ValidationResult.ValidResult;
        }
    }
}
