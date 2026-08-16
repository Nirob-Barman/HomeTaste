namespace HomeTaste.Application.Features.CategoryTypes
{
    public record CategoryTypeRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
