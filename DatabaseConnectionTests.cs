using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTest.Data;
using Microsoft.EntityFrameworkCore;

namespace TestTestUnit
{
    public class DatabaseConnectionTests
    {
        [Fact]
        public void Can_Open_Database_Connection()
        {
            // Arrange - Konfigurer en in-memory database til testen
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase") // Bruger en in-memory database
                .Options;

            using var context = new ApplicationDbContext(options);

            // Act - Prøv at åbne forbindelsen
            var canConnect = context.Database.CanConnect();

            // Assert - Forbindelsen bør kunne åbnes
            Assert.True(canConnect, "Database connection could not be established.");
        }


        [Fact]
        public void Can_Connect_To_Real_Database()
        {
            // Arrange - Brug den rigtige connection string
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-TestTest-860f5702-7d57-442c-b8ba-642ca5ddf75f;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            using var context = new ApplicationDbContext(options);

            // Act - Prøv at åbne forbindelsen
            var canConnect = context.Database.CanConnect();

            // Assert
            Assert.True(canConnect, "Database connection could not be established.");
        }

    }
}
