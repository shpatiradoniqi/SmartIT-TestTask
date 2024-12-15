using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmartIT_TestTask.Controllers;
using SmartIT_TestTask.Models;
using System.Diagnostics.Contracts;
using SmartIT_TestTask.Dtos;
using Microsoft.Extensions.Logging.Abstractions;
using SmartIT_TestTask.BackgroundProccess;
using SmartIT_TestTask.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace FacilityApi.Tests
{
    public class ContractsControllerTests
    {
        private readonly AppDbContext _dbContext;
        private readonly Mock<IBackgroundTaskQueue> _mockTaskQueue;
        private readonly Mock<ILogger<ContractProcessingService>> _mockLogger;
        private readonly ContractsController _controller;

        public ContractsControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);

            // Mock dependencies
            _mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            _mockLogger = new Mock<ILogger<ContractProcessingService>>();

            // Initialize the controller with in-memory DB and mocks
            _controller = new ContractsController(_dbContext, _mockLogger.Object, _mockTaskQueue.Object);
        }

        [Fact]
        public async Task CreateContract_ReturnsBadRequest_WhenFacilityCodeIsInvalid()
        {
            // Arrange
            var request = new CreateContractRequest
            {
                ProductionFacilityCode = "InvalidCode",
                ProcessEquipmentTypeCode = "ValidCode",
                EquipmentQuantity = 10
            };

            // Act
            var result = await _controller.CreateContract(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateContract_QueuesBackgroundTask_WhenContractIsCreated()
        {
            // Arrange
            var facility = new ProductionFacility { Code = "ValidCode", Name = "Test Facility", StandardAreaForEquipment = 100 };
            var equipmentType = new ProcessEquipmentType { Code = "EquipmentCode", Name = "Test Equipment", Area = 10 };

            _dbContext.ProductionFacilities.Add(facility);
            _dbContext.ProcessEquipmentTypes.Add(equipmentType);
            await _dbContext.SaveChangesAsync();

            var request = new CreateContractRequest
            {
                ProductionFacilityCode = facility.Code,
                ProcessEquipmentTypeCode = equipmentType.Code,
                EquipmentQuantity = 5
            };

            // Act
            var result = await _controller.CreateContract(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockTaskQueue.Verify(q => q.QueueBackgroundTask(It.IsAny<Func<CancellationToken, Task>>()), Times.Once);
        }

        
        [Fact]
        public async Task GetContracts_ReturnsListOfContracts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);
            context.ProductionFacilities.Add(new ProductionFacility
            {
                Id = 21,
                Code = "PF021",
                Name = "Facility 21",
                StandardAreaForEquipment = 1000
            });
            context.ProductionFacilities.Add(new ProductionFacility
            {
                Id = 1,
                Code = "PF001",
                Name = "Facility 1",
                StandardAreaForEquipment = 1500
            });
            context.ProcessEquipmentTypes.Add(new ProcessEquipmentType
            {
                Id = 12,
                Code = "EQ012",
                Name = "Equipment 12",
                Area = 100
            });
            context.ProcessEquipmentTypes.Add(new ProcessEquipmentType
            {
                Id = 2,
                Code = "EQ002",
                Name = "Equipment 2",
                Area = 200
            });
            context.EquipmentPlacementContracts.Add(new EquipmentPlacementContract
            {
                ProductionFacilityId = 21,
                ProcessEquipmentTypeId = 12,
                EquipmentQuantity = 2
            });
            context.EquipmentPlacementContracts.Add(new EquipmentPlacementContract
            {
                ProductionFacilityId = 1,
                ProcessEquipmentTypeId = 2,
                EquipmentQuantity = 4
            });
            await context.SaveChangesAsync();

            var mockLogger = new Mock<ILogger<ContractProcessingService>>();
            var mockTaskQueue = new Mock<IBackgroundTaskQueue>();
            //var controller = new ContractsController(context, mockLogger.Object, mockTaskQueue.Object);
            // Set up HttpContext with headers
            var httpContext = new DefaultHttpContext(); httpContext.Request.Headers["x-api-key"] = "e29177da-c81e-4d2d-b399-57db980edd5a"; var controllerContext = new ControllerContext { HttpContext = httpContext }; var controller = new ContractsController(context, mockLogger.Object, mockTaskQueue.Object) { ControllerContext = controllerContext };

            // Act
            var result = await controller.GetContracts("e29177da-c81e-4d2d-b399-57db980edd5a");
            var okResult = result.Result as OkObjectResult;
            var contracts = okResult?.Value as List<ContractResponse>;

            // Assert
            Assert.NotNull(contracts);
            Assert.Equal(2, contracts.Count);
            Assert.Contains(contracts, c => c.ProcessEquipmentTypeName == "Equipment 12" && c.ProductionFacilityName == "Facility 21");
            Assert.Contains(contracts, c => c.ProcessEquipmentTypeName == "Equipment 2" && c.ProductionFacilityName == "Facility 1");
        }


    }
}
