using CMCS.PROG6212.Data;
using Microsoft.EntityFrameworkCore;

namespace CMCS.PROG6212
{
    public class Program
    {
        public static void Main(string[] args)  // ? This is the entry point
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}



//References

//Microsoft Docs. 2025.Unit testing C# in .NET using dotnet test and xUnit. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit(Accessed: 22 October 2025). Microsoft Learn
//xUnit.net. 2025. Getting Started with xUnit.net v2. xUnit.net. Available at: https://xunit.net/docs/getting-started/v2/getting-started(Accessed: 22 October 2025). xunit.net
//Chiarelli, A. 2021. Using xUnit to Test your C# Code. Auth0 Blog. Available at: https://auth0.com/blog/xunit-to-test-csharp-code/(Accessed: 22 October 2025). Auth0
//Spasojević, M. (Code Maze). 2022. Unit Testing with xUnit in ASP.NET Core. Code Maze. Available at: https://code-maze.com/aspnetcore-unit-testing-xunit/(Accessed: 22 October 2025). Code Maze
//Bruni, O. 2025. How to Use xUnit for Unit Testing in .NET Project Using C# in VSCode. Available at: https://www.ottorinobruni.com/how-to-use-xunit-for-unit-testing-in-dotnet-project-using-csharp-in-vscode/(Accessed: 22 October 2025). Ottorino Bruni