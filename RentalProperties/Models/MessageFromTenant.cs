using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RentalProperties.Models
{
    public class MessageFromTenant
    {
        [Key]
        public int MessageId { get; set; }

        [DisplayName("Tenant")]
        public int TenantId { get; set; }
        
        [DisplayName("Tenant")]
        public virtual UserAccount? Tenant { get; set; }

        [DisplayName("Apartment")]
        public int ApartmentId { get; set; }

        [DisplayName("Apartment")]
        public virtual Apartment? Apartment { get; set; }

        [DisplayName("Message from Tenant")]
        public string MessageSent { get; set; } = null!;

        [DisplayName("Answer from Manager")]
        public string AnswerFromManager { get; set; } = null!;

    }
}
