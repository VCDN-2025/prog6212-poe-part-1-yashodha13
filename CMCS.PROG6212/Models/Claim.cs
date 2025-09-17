using System;
using System.Collections.Generic;
using System.Linq;

namespace CMCS.PROG6212.Models
{
    public class Claim
    {
        // Unique identifier for the claim
        public int ClaimId { get; set; }

        // Lecturer submitting the claim
        public int LecturerId { get; set; }

        // Month of the claim
        public DateTime Month { get; set; }

        // Status of the claim (Draft, Submitted, Pending, Approved, Rejected)
        public string Status { get; set; }

        // Comments left by the Coordinator
        public string CoordinatorComments { get; set; }

        // Comments left by the Academic Manager
        public string AcademicManagerComments { get; set; }

        // List of individual claim items (hours × rate)
        public List<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();

        // Computed property: total amount of the claim
        public decimal TotalAmount => ClaimItems?.Sum(item => item.Total) ?? 0;
    }
}
