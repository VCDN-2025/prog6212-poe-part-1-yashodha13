using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CMCS.PROG6212.Models;
using CMCS.PROG6212.Data;

// Controller and model binding patterns based on ASP.NET Core MVC documentation (Microsoft, 2024a).

namespace CMCS.PROG6212.Controllers
{
    public class LecturerController : Controller
    {
        private const int DefaultLecturerId = 101;
        private readonly string _uploadRoot = Path.Combine("wwwroot", "uploads");

        public IActionResult Dashboard(int lecturerId = DefaultLecturerId)
        {
            var claims = FakeDb.ByLecturer(lecturerId).ToList();
            ViewBag.LecturerId = lecturerId;
            return View(claims);
        }

        [HttpGet]
        public IActionResult CreateClaim()
        {
            var model = new Claim
            {
                LecturerId = DefaultLecturerId,
                // New default status in line with the workflow
                Status = "Pending Coordinator Review",
                Month = DateTime.Now,
                AcademicManagerComments = string.Empty,
                CoordinatorComments = string.Empty,
                LecturerName = "Yashodha Govender",   
                ClaimItems = new List<ClaimItem>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        // Server-side validation and ModelState usage follow ASP.NET Core model validation guidance (Microsoft, 2024b).
        public IActionResult CreateClaim(Claim claim, List<IFormFile>? attachments)
        {
            try
            {
                // Ensure lecturer id and name are always set
                if (claim.LecturerId == 0)
                    claim.LecturerId = DefaultLecturerId;

                if (string.IsNullOrWhiteSpace(claim.LecturerName))
                    claim.LecturerName = "Yashodha Govender"; // for this POE, a fixed name is fine

                // Status aligned with our workflow
                if (string.IsNullOrWhiteSpace(claim.Status))
                    claim.Status = "Pending Coordinator Review";

                claim.AcademicManagerComments ??= string.Empty;
                claim.CoordinatorComments ??= string.Empty;

                // Clean up empty claim items
                claim.ClaimItems ??= new List<ClaimItem>();
                var before = claim.ClaimItems.Count;

                claim.ClaimItems = claim.ClaimItems
                    .Where(i => !(string.IsNullOrWhiteSpace(i.Description) && i.HoursWorked == 0 && i.Rate == 0))
                    .ToList();

                if (before > claim.ClaimItems.Count)
                {
                    var keysToRemove = ModelState.Keys
                        .Where(k => k.StartsWith("ClaimItems[", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var k in keysToRemove)
                        ModelState.Remove(k);
                }

                // These are set in code, so we don't want model binding to complain
                ModelState.Remove(nameof(Claim.Status));
                ModelState.Remove(nameof(Claim.AcademicManagerComments));
                ModelState.Remove(nameof(Claim.CoordinatorComments));
                ModelState.Remove(nameof(Claim.LecturerName));

                // Revalidate the model with data annotations
                ModelState.Clear();
                TryValidateModel(claim);

                // EXTRA custom validation so you can't submit a claim
                // with no items or with zero hours/rates
                if (claim.ClaimItems == null || !claim.ClaimItems.Any())
                {
                    ModelState.AddModelError(string.Empty,
                        "Please add at least one claim item with hours worked and a rate.");
                }
                else
                {
                    for (int i = 0; i < claim.ClaimItems.Count; i++)
                    {
                        var item = claim.ClaimItems[i];

                        if (item.HoursWorked <= 0)
                        {
                            ModelState.AddModelError(
                                $"ClaimItems[{i}].HoursWorked",
                                "Hours worked must be greater than 0.");
                        }

                        if (item.Rate <= 0)
                        {
                            ModelState.AddModelError(
                                $"ClaimItems[{i}].Rate",
                                "Rate must be greater than 0.");
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    var why = string.Join("\n", ModelState
                        .Where(kv => kv.Value?.Errors?.Count > 0)
                        .Select(kv => $"{kv.Key}: {string.Join(" | ", kv.Value!.Errors.Select(e => e.ErrorMessage))}"));

                    TempData["Err"] = why;
                    ModelState.AddModelError(string.Empty, "Please fix the errors above.");
                    return View(claim);
                }

                // Save claim to in-memory DB
                var saved = FakeDb.AddClaim(claim);

                // Handle file uploads
                if (attachments != null && attachments.Count > 0)
                {
                    Directory.CreateDirectory(_uploadRoot);
                    var warnings = new List<string>();

                    foreach (var file in attachments)
                    {
                        if (file == null || file.Length == 0) continue;

                        // File type validation approach adapted from ASP.NET Core file upload examples (Microsoft, 2024c).

                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        bool allowed = ext is ".pdf" or ".docx" or ".xlsx";

                        if (!allowed)
                        {
                            warnings.Add(
                                $"Ignored file '{file.FileName}' (only .pdf, .docx, .xlsx allowed).");
                            continue;
                        }

                        var safeName = $"{Guid.NewGuid():N}{ext}";
                        var abs = Path.Combine(_uploadRoot, safeName);

                        using (var fs = new FileStream(abs, FileMode.Create))
                            file.CopyTo(fs);

                        // Save metadata using our FakeDb document support
                        var doc = new Document
                        {
                            ClaimId = saved.ClaimId,
                            OriginalFileName = file.FileName,
                            StoredFileName = safeName,
                            ContentType = file.ContentType ?? "application/octet-stream"
                        };

                        FakeDb.AddDocument(doc);
                    }

                    if (warnings.Count > 0)
                        TempData["Warn"] = string.Join(" ", warnings);
                }

                TempData["Msg"] = "Claim submitted.";
                return RedirectToAction(nameof(Dashboard), new { lecturerId = saved.LecturerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error submitting claim: " + ex.Message);
                return View(claim);
            }
        }

        // NEW – allow lecturers / admins to download documents
        [HttpGet]
        public IActionResult DownloadDocument(int id)
        {
            var doc = FakeDb.FindDocument(id);
            if (doc == null)
                return NotFound();

            var abs = Path.Combine(_uploadRoot, doc.StoredFileName);
            if (!System.IO.File.Exists(abs))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(abs);
            var contentType = string.IsNullOrWhiteSpace(doc.ContentType)
                ? "application/octet-stream"
                : doc.ContentType;

            return File(bytes, contentType, doc.OriginalFileName);
        }
    }
}




//References

// Microsoft Docs. 2025. Unit testing C# in .NET using dotnet test and xUnit. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit (Accessed: 22 October 2025).
//xUnit.net. 2025. Getting Started with xUnit.net v2. xUnit.net. Available at: https://xunit.net/docs/getting-started/v2/getting-started (Accessed: 22 October 2025).
//Chiarelli, A. 2021. Using xUnit to Test your C# Code. Auth0 Blog. Available at: https://auth0.com/blog/xunit-to-test-csharp-code (Accessed: 22 October 2025).
//Spasojević, M. 2022. Unit Testing with xUnit in ASP.NET Core. Code Maze. Available at: https://code-maze.com/aspnetcore-unit-testing-xunit (Accessed: 22 October 2025).
//Bruni, O. 2025. How to Use xUnit for Unit Testing in .NET Project Using C# in VSCode. Available at: https://www.ottorinobruni.com/how-to-use-xunit-for-unit-testing-in-dotnet-project-using-csharp-in-vscode (Accessed: 22 October 2025)./
