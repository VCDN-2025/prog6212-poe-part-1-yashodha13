using Microsoft.AspNetCore.Mvc;
using CMCS.PROG6212.Models;

namespace CMCS.PROG6212.Controllers
{
    public class LecturerController : Controller
    {
        // Sample dashboard
        public IActionResult Dashboard()
        {
            var sampleClaims = new List<Claim>
            {
                new Claim { ClaimId = 1, LecturerId = 101, Month = DateTime.Now.AddMonths(-1), Status = "Pending" },
                new Claim { ClaimId = 2, LecturerId = 101, Month = DateTime.Now, Status = "Submitted" }
            };

            return View(sampleClaims);
        }

        // GET: Create new claim
        public IActionResult CreateClaim()
        {
            return View(new Claim());
        }

        // POST: Create new claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                // For prototype, just redirect to Dashboard
                // Later you can save to DB
                return RedirectToAction("Dashboard");
            }

            // If validation fails, return same view
            return View(claim);
        }
    }
}
