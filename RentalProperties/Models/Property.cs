using Microsoft.EntityFrameworkCore;
using RentalProperties.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace RentalProperties
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [DisplayName("Name of Property")]
        public string PropertyName { get; set; } = null!;

        [DisplayName("Address: Number")]
        public string AddressNumber { get; set; } = null!;

        [DisplayName("Address: Name of Street")]
        public string AddressStreet { get; set; } = null!;

        [DisplayName("Address: Postal Code")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z] \d[A-Za-z]\d$", ErrorMessage = "Postal Code must be in the correct format: A1A 1A1.")]
        public string PostalCode { get; set; } = null!;

        [DisplayName("Address: City")]
        public string City { get; set; } = null!;

        [DisplayName("Address: Neighbourhood")]
        public string? Neighbourhood { get; set; }

        [DisplayName("Manager")]
        [Required(ErrorMessage = "Please select a manager.")]
        public int ManagerId { get; set; }

        public virtual UserAccount? Manager { get; set; }

        [NotMapped]
        public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

        [NotMapped]
        public virtual ICollection<EventInProperty> EventsInProperty { get; set; } = new List<EventInProperty>();
    }
}
