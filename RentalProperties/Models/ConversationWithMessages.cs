namespace RentalProperties.Models
{
    public class ConversationWithMessages
    {
        public ICollection<MessageFromTenant> AllMessages { get; set; }
        public MessageFromTenant newMessage { get; set; }
    }
}
