using RentalProperties.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalProperties
{
    public class UserAccount
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [DisplayName("Type of User")]
        public UserType UserType { get; set; }

        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string UserPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("UserPassword",ErrorMessage ="Password does not match!")]
        [NotMapped]
        public string UserConfirmPassword { get; set; } = null!;


        [DisplayName("Account Creation Date")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
        
        [DisplayName("First Name")]
        public string FirstName { get; set; } = null!;

        [DisplayName("Last Name")]
        public string LastName { get; set; } = null!;

        [DisplayName("Status of User")]
        public UserStatus UserStatus { get; set; }

        [NotMapped]
        [DisplayName("Remember me")]
        public bool RememberMe {  get; set; }

        [NotMapped]
        [DisplayName("Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public virtual ICollection<MessageFromTenant>? Messages { get; set; } = new List<MessageFromTenant>();
        
        [NotMapped]
        public virtual ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();

        [NotMapped]
        public virtual ICollection<Rental>? Rentals { get; set; } = new List<Rental>();
    }

    public enum UserType
    {
        PropertyOwner,
        Administrator,
        Manager,
        Tenant,
        Anonymous
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}
