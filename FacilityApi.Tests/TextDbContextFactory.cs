using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartIT_TestTask.Models;
namespace FacilityApi.Tests
{
    public static class TestDbContextFactory
    {
        public static AppDbContext CreateDbContext()
        {
            // Use a unique in-memory database name for each test run
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Unique database per test run
                .Options;

            // Create an instance of AppDbContext with the in-memory options
            var dbContext = new AppDbContext(options);

            // Seed the database with initial test data
            dbContext.ProductionFacilities.AddRange(
                new ProductionFacility { Id = 1, Name = "Main Manufacturing Plant", StandardAreaForEquipment = 5000 },
                new ProductionFacility { Id = 2, Name = "Industrial Robotics Facility", StandardAreaForEquipment = 3000 }
            );

            dbContext.ProcessEquipmentTypes.AddRange(
                new ProcessEquipmentType { Id = 1, Name = "CNC Machine", Area = 500 },
                new ProcessEquipmentType { Id = 2, Name = "Industrial Robotic Arm", Area = 300 }
            );

            // Ensure the changes are saved to the in-memory database
            dbContext.SaveChanges();
            return dbContext;
        }
    }
}

