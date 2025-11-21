using Microsoft.AspNetCore.Mvc;
using CMCS.PROG6212.Data;
using System;
using System.Linq;

namespace CMCS.PROG6212.Controllers
{
    // Coordinator review workflow implemented following ASP.NET Core MVC controller patterns (Microsoft, 2024a).
    public class CoordinatorController : Controller
    {
        public IActionResult Index()
        {
            // Only show claims that are currently waiting for the Coordinator
            var claims = FakeDb.Pending()
                .Where(c => c.Status != null &&
                            c.Status.Contains("Coordinator", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View(claims);
        }

        public IActionResult Review(int id)
        {
            var claim = FakeDb.Find(id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? comment)
        {
            // coordinator: true → updates CoordinatorApproved and status correctly
            if (!FakeDb.SetStatus(id, "Approved", comment, coordinator: true))
                return NotFound();

            TempData["Msg"] = "Claim approved by Coordinator. Sent to Academic Manager for review.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string? comment)
        {
            if (!FakeDb.SetStatus(id, "Rejected", comment, coordinator: true))
                return NotFound();

            TempData["Msg"] = "Claim rejected by Coordinator.";
            return RedirectToAction(nameof(Index));
        }
    }
}


//References

// Microsoft Docs. 2025. Unit testing C# in .NET using dotnet test and xUnit. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit (Accessed: 22 October 2025).
//xUnit.net. 2025. Getting Started with xUnit.net v2. xUnit.net. Available at: https://xunit.net/docs/getting-started/v2/getting-started (Accessed: 22 October 2025).
//Chiarelli, A. 2021. Using xUnit to Test your C# Code. Auth0 Blog. Available at: https://auth0.com/blog/xunit-to-test-csharp-code (Accessed: 22 October 2025).
//Spasojević, M. 2022. Unit Testing with xUnit in ASP.NET Core. Code Maze. Available at: https://code-maze.com/aspnetcore-unit-testing-xunit (Accessed: 22 October 2025).
//Bruni, O. 2025. How to Use xUnit for Unit Testing in .NET Project Using C# in VSCode. Available at: https://www.ottorinobruni.com/how-to-use-xunit-for-unit-testing-in-dotnet-project-using-csharp-in-vscode (Accessed: 22 October 2025)./
