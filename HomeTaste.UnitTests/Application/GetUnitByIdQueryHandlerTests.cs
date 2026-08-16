using System.Linq.Expressions;
using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Features.Units.Queries.GetUnitById;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Domain.Entities;
using Moq;

namespace HomeTaste.UnitTests.Application
{
    public class GetUnitByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly GetUnitByIdQueryHandler _handler;

        public GetUnitByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new GetUnitByIdQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WhenUnitExists_ReturnsUnit()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            _mockUnitOfWork
                .Setup(u => u.Repository<Units>().GetByIdAsync(unitId, It.IsAny<Expression<Func<Units, UnitResponse>>>()))
                .ReturnsAsync(new UnitResponse { Id = unitId, Name = "Kilogram", Abbreviation = "kg" });

            // Act
            var result = await _handler.Handle(new GetUnitByIdQuery(unitId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Kilogram", result.Data!.Name);
            Assert.Equal("kg", result.Data.Abbreviation);
        }

        [Fact]
        public async Task Handle_WhenUnitNotFound_ReturnsFailure()
        {
            // Arrange
            var unitId = Guid.NewGuid();
            _mockUnitOfWork
                .Setup(u => u.Repository<Units>().GetByIdAsync(unitId, It.IsAny<Expression<Func<Units, UnitResponse>>>()))
                .ReturnsAsync((UnitResponse?)null);

            // Act
            var result = await _handler.Handle(new GetUnitByIdQuery(unitId), CancellationToken.None);

            // Assert
            Assert.False(result.Success);
        }
    }
}
