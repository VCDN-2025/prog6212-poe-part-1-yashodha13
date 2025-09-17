using System;

namespace CMCS.PROG6212.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int ClaimId { get; set; }
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
