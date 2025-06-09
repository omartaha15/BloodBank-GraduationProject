using BloodBank.Core.Entities;
using System;
using System.Collections.Generic;

namespace Blood_Bank.ViewModels
{
    public class ReportViewModel
    {
        // Date range for the report
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Period { get; set; }

        // Summary metrics
        public int TotalDonations { get; set; }
        public int TotalBloodUnits { get; set; }
        public int TotalPassedTests { get; set; }
        public int TotalFailedTests { get; set; }
        public int TotalRequests { get; set; }
        public int FulfilledRequests { get; set; }

        // Data collections
        public List<Donation> Donations { get; set; } = new List<Donation>();
        public List<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
        public List<BloodTest> BloodTests { get; set; } = new List<BloodTest>();
        public List<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

        // Chart data (serialized JSON strings)
        public string BloodTypeLabels { get; set; }
        public string BloodTypeCounts { get; set; }
    }
}