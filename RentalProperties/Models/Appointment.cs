using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RentalProperties.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [DisplayName("Tenant")]
        public int TenantId { get; set; }
        
        [DisplayName("Tenant")]
        public virtual UserAccount? Tenant { get; set; }

        [DisplayName("Apartment")]
        public int ApartmentId { get; set; }

        [DisplayName("Apartment")]
        public virtual Apartment? Apartment { get; set; }

        [DisplayName("Date of Visit")]
        public DateOnly VisitDate { get; set; }

    }
}
