using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Pivec.Promotion.Data
{
    [MetadataType(typeof(PromotionMetadata))]
    public partial class Promotion
    {
        private class PromotionMetadata
        {
            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public Guid Id { get; set; }

            [Display(Name = "Promotion Name")]
            [Required]
            public string Name { get; set; }
        }
    }
}