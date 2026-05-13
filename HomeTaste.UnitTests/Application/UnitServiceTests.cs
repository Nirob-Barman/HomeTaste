
using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Services.Measurements;
using HomeTaste.Domain.Entities;
using Moq;

namespace HomeTaste.UnitTests.Application
{
    public class UnitServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UnitService _unitService;

        public UnitServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _unitService = new UnitService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetUnitById_ShouldReturnUnit_WhenFound()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            var expectedUnitResponse = new UnitResponse
            {
                Id = unitId,
                Name = "Kilogram",
                Abbreviation = "kg"
            };

            _mockUnitOfWork.Setup(u => u.Repository<Units>().GetByIdAsync(unitId))
                .ReturnsAsync(new Units
                {
                    Id = unitId,
                    Name = "Kilogram",
                    Abbreviation = "kg"
                });

            // Act
            var result = await _unitService.GetUnitByIdAsync(unitId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Kilogram", result.Data!.Name);
            Assert.Equal("kg", result.Data.Abbreviation);
        }

    }
}
