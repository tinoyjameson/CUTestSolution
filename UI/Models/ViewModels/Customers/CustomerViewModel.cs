using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UI.Models.ViewModels.Customers
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }
        [DisplayName("Company Registration Number")]
        public string CompanyRegistrationNumber { get; set; }

        [DisplayName("Incorporation Date")]
        [DataType(DataType.Date)]
        public DateTime IncorporationDate { get; set; }

        [DisplayName("Turnover")]
        public double TurnOver { get; set; }
        public bool IsActive { get; set; }
    }
}