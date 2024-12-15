﻿namespace SmartIT_TestTask.Models
{
    public class ProductionFacility
    {
        public int Id { get; set; } // Primary Key
        public string Code { get; set; } = string.Empty; // Unique Code
        public string Name { get; set; } = string.Empty;
        public double StandardAreaForEquipment { get; set; }

        // Navigation Property
        public ICollection<EquipmentPlacementContract> Contracts { get; set; } = new List<EquipmentPlacementContract>();
    }
}
