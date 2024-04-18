using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalProperties.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }

        [DisplayName("Tenant")]
        public int TenantId { get; set; }

        [DisplayName("Tenant")]
        public virtual UserAccount? Tenant { get; set; }

        [DisplayName("Apartment")]
        public int ApartmentId { get; set; }

        [DisplayName("Apartment")]
        public virtual Apartment? Apartment { get; set; }

        [NotMapped]
        public virtual ICollection<MessageFromTenant>? Messages { get; set; } = new List<MessageFromTenant>();

    }
}
