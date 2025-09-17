namespace CMCS.PROG6212.Models
{
    public class ClaimItem
    {
        public int ClaimItemId { get; set; }       // Primary key
        public int ClaimId { get; set; }           // Foreign key to Claim
        public string Description { get; set; }    // Task/Work description
        public int HoursWorked { get; set; }       // Number of hours
        public decimal Rate { get; set; }          // Hourly rate

        // Computed property: total amount for this item
        public decimal Total => HoursWorked * Rate;
    }
}
