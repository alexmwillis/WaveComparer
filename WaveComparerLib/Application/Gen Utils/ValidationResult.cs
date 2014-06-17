using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Gen_Utils
{
    public class ValidationResult
    {
        public ValidationResult(bool isValid, List<string> errors)
        {
            this.IsValid = isValid;
            this.Errors = errors;
        }

        public ValidationResult(bool isValid, string error)
        {
            var errors = new List<string>();
            errors.Add(error);

            this.IsValid = isValid;
            this.Errors = errors;
        }
        
        public readonly List<string> Errors;
        public readonly bool IsValid;
    }
}
