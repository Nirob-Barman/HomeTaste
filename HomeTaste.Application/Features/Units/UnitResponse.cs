namespace HomeTaste.Application.Features.Units
{
    public record UnitResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
    }
}
