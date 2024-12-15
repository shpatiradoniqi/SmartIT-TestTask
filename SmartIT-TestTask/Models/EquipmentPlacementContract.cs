namespace SmartIT_TestTask.Models
{
    public class EquipmentPlacementContract
    {
        public int Id { get; set; } // Primary Key
        public int ProductionFacilityId { get; set; }
        public int ProcessEquipmentTypeId { get; set; }
        public int EquipmentQuantity { get; set; }

        // Navigation Properties
        public ProductionFacility ProductionFacility { get; set; } = null!;
        public ProcessEquipmentType ProcessEquipmentType { get; set; } = null!;
    }
}
