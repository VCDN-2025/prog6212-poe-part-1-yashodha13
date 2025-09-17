using Microsoft.AspNetCore.Mvc;
using CMCS.PROG6212.Models;

namespace CMCS.PROG6212.Controllers
{
    public class CoordinatorController : Controller
    {
        public IActionResult Index()
        {
            var claims = new List<Claim>
            {
                new Claim { ClaimId = 3, LecturerId = 102, Month = DateTime.Now, Status = "Pending Review" }
            };

            return View(claims);
        }

        public IActionResult Review(int id)
        {
            var claim = new Claim
            {
                ClaimId = id,
                LecturerId = 102,
                Month = DateTime.Now,
                Status = "Pending Review"
            };

            return View(claim);
        }
    }
}
