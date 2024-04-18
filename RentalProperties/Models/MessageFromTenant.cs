using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RentalProperties.Models
{
    public class MessageFromTenant
    {
        [Key]
        public int MessageId { get; set; }

        [DisplayName("Conversation")]
        public int ConversationId {  get; set; }

        [DisplayName("Conversation")]
        public virtual Conversation? Conversation { get; set; }

        [DisplayName("Message")]
        public string? MessageSent { get; set; } = null!;
        
        [DisplayName("Date Sent")]
        public DateTime DateSent { get; set; }

        [DisplayName("Type of Author")] 
        public UserType AuthorType { get; set; }

        [DisplayName("Name of Author")]
        public string? AuthorName {  get; set; }
    }
}

