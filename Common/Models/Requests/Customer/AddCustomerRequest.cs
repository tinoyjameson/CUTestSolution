﻿using System;

namespace Common.Models.Requests.Customer
{
    public class AddCustomerRequest
    {
        public string Name { get; set; }

        public string CompanyRegistrationNumber { get; set; }
        public DateTime IncorporationDate { get; set; }
        public double TurnOver { get; set; }
        public bool IsActive { get; set; }
    }
}