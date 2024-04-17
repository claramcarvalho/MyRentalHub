using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RentalProperties.Models
{
    public class ManagerAvailability
    {
        [Key]
        public int ManagerAvailabilityId { get; set; }

        [DisplayName("Manager")]
        public int ManagerId { get; set; }
        
        [DisplayName("Manager")]
        public virtual UserAccount? Manager { get; set; }

        [DisplayName("Availability (1-hour slot)")]
        public DateTime AvailableSlot { get; set; }
    }
}
