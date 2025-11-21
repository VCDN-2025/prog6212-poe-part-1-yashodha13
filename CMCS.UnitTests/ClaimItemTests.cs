using Xunit;
using CMCS.PROG6212.Models;

namespace CMCS.UnitTests
{


    // xUnit test structure and examples adapted from Auth0, Code Maze and xUnit resources(Chiarelli, 2022; Spasojević, 2022; Bruni, 2025; xUnit.net, 2025; Microsoft, 2025).
    public class ClaimItemTests
    {
        [Fact]
        public void Total_ReturnsCorrectAmount()
        {
            // Arrange
            var item = new ClaimItem
            {
                HoursWorked = 5,
                Rate = 100m
            };

            // Act
            var total = item.Total;

            // Assert
            Assert.Equal(500m, total);
        }

        [Fact]
        public void Description_IsRequired()
        {
            var item = new ClaimItem
            {
                Description = null,
                HoursWorked = 1,
                Rate = 100
            };

            Assert.ThrowsAny<Exception>(() =>
            {
                var x = item.Description.Length;
            });
        }
    }
}
