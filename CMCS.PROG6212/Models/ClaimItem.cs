using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.PROG6212.Models
{
    // Validation attributes adapted from ASP.NET Core model validation examples (Microsoft, 2024b).
    public class ClaimItem
    {
        public int ClaimItemId { get; set; }
        public int ClaimId { get; set; }

        // Description is required
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        // Hours must be > 0
        [Range(1, 200, ErrorMessage = "Hours worked must be greater than 0.")]
        public int HoursWorked { get; set; }

        // Rate must be > 0
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 2000, ErrorMessage = "Rate must be greater than 0.")]
        public decimal Rate { get; set; }

        [NotMapped]
        public decimal Total => HoursWorked * Rate;
    }
}

//References

// Microsoft Docs. 2025. Unit testing C# in .NET using dotnet test and xUnit. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit (Accessed: 22 October 2025).
//xUnit.net. 2025. Getting Started with xUnit.net v2. xUnit.net. Available at: https://xunit.net/docs/getting-started/v2/getting-started (Accessed: 22 October 2025).
//Chiarelli, A. 2021. Using xUnit to Test your C# Code. Auth0 Blog. Available at: https://auth0.com/blog/xunit-to-test-csharp-code (Accessed: 22 October 2025).
//Spasojević, M. 2022. Unit Testing with xUnit in ASP.NET Core. Code Maze. Available at: https://code-maze.com/aspnetcore-unit-testing-xunit (Accessed: 22 October 2025).
//Bruni, O. 2025. How to Use xUnit for Unit Testing in .NET Project Using C# in VSCode. Available at: https://www.ottorinobruni.com/how-to-use-xunit-for-unit-testing-in-dotnet-project-using-csharp-in-vscode (Accessed: 22 October 2025)./

