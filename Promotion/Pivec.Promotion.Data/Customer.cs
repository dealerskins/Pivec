using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Pivec.Promotion.Data
{
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
        private class CustomerMetadata
        {
            public Guid Id { get; set; }

            [Required]
            public Guid PromotionId { get; set; }

            [Required]
            public Guid DealerId { get; set; }

            [Display(Name = "Salesperson Code")]
            [Required]
            [RegularExpression(@"^\d*$")]
            public string SalespersonCode { get; set; }

            [Display(Name = "Driver License Number")]
            [Required]
            public string DriverLicenseNumber { get; set; }

            [Display(Name = "First Name")]
            [Required]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            [Required]
            public string LastName { get; set; }

            [Display(Name = "Street Address")]
            [Required]
            public string StreetAddress { get; set; }

            [Display(Name = "City")]
            [Required]
            public string City { get; set; }

            [Display(Name = "State")]
            [Required]
            public string State { get; set; }

            [Display(Name = "Zip Code")]
            [Required]
            [RegularExpression(@"(^\d{5}$)|(^\d{5}-\d{4}$)", ErrorMessage = "Zip Code is invalid.")]
            public string ZipCode { get; set; }

            [Display(Name = "eMail")]
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Display(Name = "Creation Date")]
            public DateTime DateCreated { get; set; }
        }
    }
}
