using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Validations
{
    /// <summary>
    /// A custom Attribute Validation to ensure the first letter is uppercase
    /// </summary>
    public class FirstLetterUppercaseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                // If the string given is empty, return success. No need to check it
                return ValidationResult.Success;
            }

            var firstLetter = value.ToString()[0].ToString();

            if (firstLetter != firstLetter.ToUpper())
            {
                // If the first letter isn't uppercase, return an error
                return new ValidationResult("First letter should be uppercase!");
            }

            return ValidationResult.Success;
        }
    }
}