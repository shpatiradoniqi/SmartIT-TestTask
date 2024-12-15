namespace SmartIT_TestTask.Dtos
{
    public class CreateContractRequest
    {
        public string ProductionFacilityCode { get; set; } = string.Empty;
        public string ProcessEquipmentTypeCode { get; set; } = string.Empty;
        public int EquipmentQuantity { get; set; }
    }
}
