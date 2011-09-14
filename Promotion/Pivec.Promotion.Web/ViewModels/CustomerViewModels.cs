using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Pivec.Promotion.Data;

namespace Pivec.Promotion.Web.ViewModels
{

    public class CustomerSearchViewModel
    {
        [Display(Name = "Dealer")]
        public SelectList DealerId { get; set; }

        [Display(Name = "Salesperson Code")]
        [RegularExpression(@"^[a-zA-Z0-9\-]*$", ErrorMessage = "Sales person code is invalid.")]
        public string SalespersonCode { get; set; }

        [Display(Name = "Date From")]
        [DataType(DataType.Date, ErrorMessage = "Date From is invalid.")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date, ErrorMessage = "Date To is invalid.")]
        public DateTime? DateTo { get; set; }

        public IEnumerable<Customer> Customers { get; set; }
    }

    public class CustomerExcelViewModel
    {
        public string fileName { get; set; }
        public IEnumerable<Customer> contents { get; set; }
    }

}