using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Pivec.Promotion.Data
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        private class UserMetadata
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }

            [Display(Name = "Login")]
            [Required]
            public string Login { get; set; }

            [Display(Name = "Password")]
            [Required]
            public string Password { get; set; }

            [Display(Name = "Administrator")]
            [Required]
            public bool IsAdmin { get; set; }
        }
    }
}