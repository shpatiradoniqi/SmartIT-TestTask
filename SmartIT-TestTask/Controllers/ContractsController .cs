using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartIT_TestTask.BackgroundProccess;
using SmartIT_TestTask.Dtos;
using SmartIT_TestTask.Models;
using SmartIT_TestTask.Services;

namespace SmartIT_TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ContractProcessingService> _logger;
        private const string ApiKey = "e29177da-c81e-4d2d-b399-57db980edd5a";

        public ContractsController(AppDbContext context,  ILogger<ContractProcessingService> logger ,IBackgroundTaskQueue taskQueue)
        {
            _context = context;
            _taskQueue = taskQueue;
            _logger = logger;
           
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract(CreateContractRequest request)
        {
            var facility = await _context.ProductionFacilities
                .FirstOrDefaultAsync(f => f.Code == request.ProductionFacilityCode);
            if (facility == null)
                return BadRequest("Invalid Production Facility Code.");

            var equipmentType = await _context.ProcessEquipmentTypes
                .FirstOrDefaultAsync(e => e.Code == request.ProcessEquipmentTypeCode);
            if (equipmentType == null) 
                return BadRequest("Invalid Process Equipment Type Code.");

            double requiredArea = equipmentType.Area * request.EquipmentQuantity;
            double usedArea = _context.EquipmentPlacementContracts
                .Where(c => c.ProductionFacilityId == facility.Id)
                .Sum(c => c.EquipmentQuantity * c.ProcessEquipmentType.Area);

            if (usedArea + requiredArea > facility.StandardAreaForEquipment)
                return BadRequest("Not enough space in the Production Facility.");

            var contract = new EquipmentPlacementContract
            {
                ProductionFacilityId = facility.Id,
                ProcessEquipmentTypeId = equipmentType.Id,
                EquipmentQuantity = request.EquipmentQuantity
            };

            _context.EquipmentPlacementContracts.Add(contract);
            await _context.SaveChangesAsync();

            // Queue Background Task
            _taskQueue.QueueBackgroundTask(async token =>
            {
                await Task.Delay(1000, token); // Simulate work
                _logger.LogInformation($"Contract created for Facility: {facility.Name}, Equipment Type: {equipmentType.Name}");
            });

            return Ok("Contract Created Successfully!"); 
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractResponse>>> GetContracts([FromQuery] string apiKey)
        {
            // Validate the API key
            if (string.IsNullOrEmpty(apiKey) || apiKey != ApiKey)
            {
                return Unauthorized(new { message = "Invalid or missing API key" });
            }

            var contracts = await _context.EquipmentPlacementContracts
                .Include(c => c.ProductionFacility)
                .Include(c => c.ProcessEquipmentType)
                .Select(c => new ContractResponse
                {
                    ProductionFacilityName = c.ProductionFacility.Name,
                    ProcessEquipmentTypeName = c.ProcessEquipmentType.Name,
                    EquipmentQuantity = c.EquipmentQuantity
                })
                .ToListAsync();

            return Ok(contracts);
        }
    }
}
