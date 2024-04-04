using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RentalProperties.Models
{
    public class EventInProperty
    {
        [Key]
        public int EventId { get; set; }

        [DisplayName("Property")]
        public int PropertyId { get; set; }
        
        [DisplayName("Property")]
        public virtual Property? Property { get; set; }

        [DisplayName("Apartment")]
        public int? ApartmentId { get; set; }
        
        [DisplayName("Apartment")]
        public virtual Apartment? Apartment { get; set; }

        [DisplayName("Event: Title")]
        public string EventTitle { get; set; } = null!;

        [DisplayName("Description of the event")]
        public string? EventDescription { get; set; }

        [DisplayName("Reported Date")]
        public DateOnly ReportDate { get; set; }

    }
}
