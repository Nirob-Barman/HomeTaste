using HomeTaste.Application.Features.Units.Queries.GetUnitById;
using HomeTaste.Domain.Entities;

namespace HomeTaste.UnitTests.Application
{
    public class GetUnitByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_WhenUnitExists_ReturnsUnit()
        {
            // Arrange
            using var context = TestApplicationDbContext.CreateInMemory();
            var unitId = Guid.NewGuid();
            var unit = Units.Create("Kilogram", "kg");
            unit.Id = unitId;
            context.Units.Add(unit);
            await context.SaveChangesAsync();

            var handler = new GetUnitByIdQueryHandler(context);

            // Act
            var result = await handler.Handle(new GetUnitByIdQuery(unitId), CancellationToken.None);

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
            using var context = TestApplicationDbContext.CreateInMemory();
            var handler = new GetUnitByIdQueryHandler(context);

            // Act
            var result = await handler.Handle(new GetUnitByIdQuery(Guid.NewGuid()), CancellationToken.None);

            // Assert
            Assert.False(result.Success);
        }
    }
}
