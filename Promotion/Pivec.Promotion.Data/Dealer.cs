using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Pivec.Promotion.Data
{
    [MetadataType(typeof(DealerMetadata))]
    public partial class Dealer
    {
        private class DealerMetadata
        {
            public Guid Id { get; set; }

            [Display(Name = "Dealer Name")]
            [Required]
            public string Name { get; set; }
        }
    }
}