using System;
using System.Collections.Generic;
using System.Linq;
using CMCS.PROG6212.Models;

namespace CMCS.PROG6212.Data
{
    /// <summary>
    /// In-memory claim store inspired by repository patterns used in ASP.NET Core samples (Microsoft, 2024a).
    /// </summary>
    
    public static class FakeDb

    {
        private static readonly object _lock = new();

        // If your Part 1 sample data used ClaimIds 1..2 in views, start from 3.
        private static int _nextClaimId = 3;

        // NEW – auto increment for documents
        private static int _nextDocumentId = 1;

        public static List<Claim> Claims { get; } = new();

        // NEW – central store for uploaded documents
        public static List<Document> Documents { get; } = new();

        public static Claim AddClaim(Claim c)
        {
            lock (_lock)
            {
                c.ClaimId = _nextClaimId++;

                // Default status for a new claim – waiting on coordinator first
                if (string.IsNullOrWhiteSpace(c.Status))
                    c.Status = "Pending Coordinator Review";

                if (c.Month == default)
                    c.Month = DateTime.Now;

                // Ensure navigation lists are never null
                c.ClaimItems ??= new List<ClaimItem>();
                c.Documents ??= new List<Document>();

                Claims.Add(c);
                return c;
            }
        }

        public static IEnumerable<Claim> ByLecturer(int lecturerId)
        {
            lock (_lock)
            {
                return Claims
                    .Where(x => x.LecturerId == lecturerId)
                    .OrderByDescending(x => x.Month)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns all claims that are in any "pending" state.
        /// Coordinator and Academic Manager controllers can still
        /// filter further if needed.
        /// </summary>
        public static IEnumerable<Claim> Pending()
        {
            lock (_lock)
            {
                return Claims
                    .Where(x =>
                        x.Status != null &&
                        x.Status.StartsWith("Pending", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.Month)
                    .ToList();
            }
        }

        public static Claim? Find(int id)
        {
            lock (_lock)
            {
                return Claims.FirstOrDefault(x => x.ClaimId == id);
            }
        }

        // ---------------- DOCUMENT HANDLING (Part 3) ----------------

        public static Document AddDocument(Document doc)
        {
            lock (_lock)
            {
                doc.DocumentId = _nextDocumentId++;

                Documents.Add(doc);

                var claim = Claims.FirstOrDefault(c => c.ClaimId == doc.ClaimId);
                if (claim != null)
                {
                    claim.Documents ??= new List<Document>();
                    claim.Documents.Add(doc);
                }

                return doc;
            }
        }

        public static IEnumerable<Document> GetDocumentsForClaim(int claimId)
        {
            lock (_lock)
            {
                return Documents
                    .Where(d => d.ClaimId == claimId)
                    .ToList();
            }
        }

        public static Document? FindDocument(int documentId)
        {
            lock (_lock)
            {
                return Documents.FirstOrDefault(d => d.DocumentId == documentId);
            }
        }

        // ---------------- APPROVAL WORKFLOW (Part 3) ----------------

        /// <summary>
        /// Sets status and (optionally) stores a reviewer comment.
        /// Also updates CoordinatorApproved / AcademicManagerApproved
        /// so that BOTH must approve before the claim is fully approved.
        /// </summary>
        public static bool SetStatus(int id, string status, string? comment = null, bool coordinator = true)
        {
            lock (_lock)
            {
                var c = Claims.FirstOrDefault(x => x.ClaimId == id);
                if (c == null) return false;

                // APPROVAL PATH
                if (status == "Approved")
                {
                    if (coordinator)
                    {
                        c.CoordinatorApproved = true;

                        if (!string.IsNullOrWhiteSpace(comment))
                            c.CoordinatorComments = comment;

                        // After coordinator approval: either final approved
                        // (if manager already approved) or now pending manager
                        c.Status = c.AcademicManagerApproved
                            ? "Approved"
                            : "Pending Academic Manager Review";
                    }
                    else
                    {
                        c.AcademicManagerApproved = true;

                        if (!string.IsNullOrWhiteSpace(comment))
                            c.AcademicManagerComments = comment;

                        // After manager approval: either final approved
                        // (if coordinator already approved) or still pending them
                        c.Status = c.CoordinatorApproved
                            ? "Approved"
                            : "Pending Coordinator Review";
                    }
                }
                else
                {
                    // REJECTIONS OR OTHER MANUAL STATUSES
                    c.Status = status;

                    if (coordinator)
                    {
                        if (!string.IsNullOrWhiteSpace(comment))
                            c.CoordinatorComments = comment;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(comment))
                            c.AcademicManagerComments = comment;
                    }
                }

                return true;
            }
        }
    }
}


//References

// Microsoft Docs. 2025. Unit testing C# in .NET using dotnet test and xUnit. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit (Accessed: 22 October 2025).
//xUnit.net. 2025. Getting Started with xUnit.net v2. xUnit.net. Available at: https://xunit.net/docs/getting-started/v2/getting-started (Accessed: 22 October 2025).
//Chiarelli, A. 2021. Using xUnit to Test your C# Code. Auth0 Blog. Available at: https://auth0.com/blog/xunit-to-test-csharp-code (Accessed: 22 October 2025).
//Spasojević, M. 2022. Unit Testing with xUnit in ASP.NET Core. Code Maze. Available at: https://code-maze.com/aspnetcore-unit-testing-xunit (Accessed: 22 October 2025).
//Bruni, O. 2025. How to Use xUnit for Unit Testing in .NET Project Using C# in VSCode. Available at: https://www.ottorinobruni.com/how-to-use-xunit-for-unit-testing-in-dotnet-project-using-csharp-in-vscode (Accessed: 22 October 2025)./
