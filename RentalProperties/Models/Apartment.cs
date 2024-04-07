using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalProperties.Models
{
    public class Apartment
    {
        [Key]
        public int ApartmentId { get; set; }

        [DisplayName("Property")]
        [Required(ErrorMessage = "Please select a property.")]
        public int PropertyId { get; set; }
        
        [DisplayName("Property")]
        public virtual Property? Property { get; set; } = null!;

        [DisplayName("Apartment Number")]
        public string ApartmentNumber { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Number of bedrooms must be non-negative.")]
        [DisplayName("Number of Beds")]
        public int NbOfBeds { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Number of bathrooms must be at least one.")]
        [DisplayName("Number of Bathrooms")]
        public int NbOfBaths { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of parking spots must be non-negative.")]
        [DisplayName("Number of Parking Spots")]
        public int NbOfParkingSpots { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        [DisplayName("Price of Announced Rent")]
        public decimal PriceAnnounced { get; set; }

        [DisplayName("Animals Accepted")]
        public bool AnimalsAccepted { get; set; }

        [NotMapped]
        public virtual ICollection<MessageFromTenant> Messages { get; set; } = new List<MessageFromTenant>();
        
        [NotMapped]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        [NotMapped]
        public virtual ICollection<EventInProperty> EventsInApartment { get; set; } = new List<EventInProperty>();

        [NotMapped]
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();


    }
}
