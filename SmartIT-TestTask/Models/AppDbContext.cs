using Microsoft.EntityFrameworkCore;

namespace SmartIT_TestTask.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ProductionFacility> ProductionFacilities { get; set; }
        public DbSet<ProcessEquipmentType> ProcessEquipmentTypes { get; set; }
        public DbSet<EquipmentPlacementContract> EquipmentPlacementContracts { get; set; }

    }
}
