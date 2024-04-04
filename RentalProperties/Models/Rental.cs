using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalProperties.Models
{
    public class Rental
    {
        [Key]
        [Column (Order = 1)]
        [DisplayName("Tenant")]
        public int TenantId { get; set; }

        [DisplayName("Tenant")]
        public virtual UserAccount? Tenant { get; set; }

        [Key]
        [Column(Order = 2)]
        [DisplayName("Apartment")]
        public int ApartmentId { get; set; }

        [DisplayName("Apartment")]
        public virtual Apartment? Apartment { get; set; }

        [Key]
        [Column(Order = 3)]
        [DataType(DataType.Date)]
        [DisplayName("First Day of Contract")]
        public DateOnly FirstDayRental { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Last Day of Contract")]
        public DateOnly LastDayRental { get; set; }

        [DisplayName("Price of Actual Rent")]
        public decimal PriceRent { get; set; }

        [DisplayName("Status of Contract")]
        public virtual StatusOfRental RentalStatus { get; set; }
    }

    public enum StatusOfRental
    {
        Active,
        Scheduled,
        Pending,
        Expired,
        Terminated,
        RenewalPeriod
    }
}
