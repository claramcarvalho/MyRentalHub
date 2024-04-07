using RentalProperties.Models;
using System.ComponentModel.DataAnnotations;

namespace RentalProperties.Validation
{
    public class ValidationLastDayRentalAfterFirstDayRental : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var rental = (Rental)validationContext.ObjectInstance;

            if (rental.LastDayRental < rental.FirstDayRental)
            {
                return new ValidationResult("Last Day of Contract must be on or after First Day of Contract");
            }

            return ValidationResult.Success;
        }
    }
}
