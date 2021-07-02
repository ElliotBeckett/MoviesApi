using MoviesApi.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class Genre //: IValidatableObject // Interface to create model based validation
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "The field with name {0} is required")] // Makes this a required field - with a custom error message
        [StringLength(10)] // Sets the min & max length a string/value can be
        [FirstLetterUppercase] // Custom attribute validation - This can be used anywhere on any model.
        public string Name { get; set; }

        /*
         * Additional checks that can be added with object attributes.
         *
         *  [Range(18,120)]
         *  public int Age { get; set; }
         *  [CreditCard]
         *  public string CreditCard { get; set; }
         *  [Url]
         *  public string Url { get; set; }
         *
         */

        // Custom Model Validation - Hard coded to the model and can't be reused elsewhere.

        /* public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
         {
             // Check to make sure the First letter in the Name attribute is uppercase
             if (!string.IsNullOrEmpty(Name))
             {
                 var firstLetter = Name[0].ToString();

                 if (firstLetter != firstLetter.ToUpper())
                 {
                     yield return new ValidationResult("First letter should be uppercase!", new string[] { nameof(Name) });
                 }
             }
         }*/
    }
}