using Microsoft.EntityFrameworkCore;
using RentalProperties.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalProperties.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }
        
        [DisplayName("Tenant")]
        public int TenantId { get; set; }

        [DisplayName("Tenant")]
        public virtual UserAccount? Tenant { get; set; }

        [DisplayName("Apartment")]
        public int ApartmentId { get; set; }

        [DisplayName("Apartment")]
        public virtual Apartment? Apartment { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("First Day of Contract")]
        public DateOnly FirstDayRental { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Last Day of Contract")]
        [ValidationLastDayRentalAfterFirstDayRental(ErrorMessage = "Last Day of Contract must be on or after First Day of Contract")]
        public DateOnly LastDayRental { get; set; }

        [DisplayName("Price of Actual Rent")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        public decimal PriceRent { get; set; }

        [DisplayName("Status of Contract")]
        public virtual StatusOfRental RentalStatus { get; set; }
    }

    public enum StatusOfRental
    {
        Signed,
        Pending,
        Terminated
    }
}
