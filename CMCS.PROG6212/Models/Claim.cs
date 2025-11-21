using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CMCS.PROG6212.Models
{
    // Validation attributes adapted from ASP.NET Core model validation examples (Microsoft, 2024b).
    public class Claim
    {
        public int ClaimId { get; set; }

        // Lecturer must be known
        [Required(ErrorMessage = "Lecturer ID is required.")]
        public int LecturerId { get; set; }

        // Will display in the views (you can default it if you like)
        [Required(ErrorMessage = "Lecturer name is required.")]
        public string LecturerName { get; set; }

        // Month must be chosen on the form
        [Required(ErrorMessage = "Please select a month for this claim.")]
        public DateTime Month { get; set; }

        public string Status { get; set; }
        public string CoordinatorComments { get; set; }
        public string AcademicManagerComments { get; set; }

        // Separate approval flags
        public bool CoordinatorApproved { get; set; }
        public bool AcademicManagerApproved { get; set; }

        // At least one claim item must be present after cleaning
        [Required(ErrorMessage = "Please add at least one claim item.")]
        [MinLength(1, ErrorMessage = "Please add at least one claim item.")]
        public List<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();

        // Uploaded documents linked to this claim
        public List<Document> Documents { get; set; } = new List<Document>();

        public decimal TotalAmount => ClaimItems?.Sum(item => item.Total) ?? 0;
    }
}

//References

// Microsoft Docs. 2025. Unit testing C# in .NET using dotnet test and xUnit. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit (Accessed: 22 October 2025).
//xUnit.net. 2025. Getting Started with xUnit.net v2. xUnit.net. Available at: https://xunit.net/docs/getting-started/v2/getting-started (Accessed: 22 October 2025).
//Chiarelli, A. 2021. Using xUnit to Test your C# Code. Auth0 Blog. Available at: https://auth0.com/blog/xunit-to-test-csharp-code (Accessed: 22 October 2025).
//Spasojević, M. 2022. Unit Testing with xUnit in ASP.NET Core. Code Maze. Available at: https://code-maze.com/aspnetcore-unit-testing-xunit (Accessed: 22 October 2025).
//Bruni, O. 2025. How to Use xUnit for Unit Testing in .NET Project Using C# in VSCode. Available at: https://www.ottorinobruni.com/how-to-use-xunit-for-unit-testing-in-dotnet-project-using-csharp-in-vscode (Accessed: 22 October 2025)./

