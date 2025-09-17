using Microsoft.AspNetCore.Mvc;
using CMCS.PROG6212.Models;

namespace CMCS.PROG6212.Controllers
{
    public class AcademicManagerController : Controller
    {
        // Temporary in-memory list (replace with DB later)
        private static List<Claim> claims = new List<Claim>
        {
            new Claim { ClaimId = 5, LecturerId = 103, Month = DateTime.Now, Status = "Awaiting Approval" }
        };

        public IActionResult Index()
        {
            return View(claims);
        }

        // ✅ Approve action
        public IActionResult Approve(int id)
        {
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = "Approved";
            }

            return RedirectToAction("Index");
        }

        // ✅ Reject action
        public IActionResult Reject(int id)
        {
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
            }

            return RedirectToAction("Index");
        }
    }
}

