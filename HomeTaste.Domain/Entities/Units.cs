namespace HomeTaste.Domain.Entities
{
    public class Units : BaseEntity
    {
        public Guid Id { get; set; }               // Unique Identifier for the Unit (Primary Key)
        public string? Name { get; set; }           // Name of the Unit (e.g., Kilogram, Gram, Liter, Piece)
        public string? Abbreviation { get; set; }   // Abbreviation for the Unit (e.g., kg, g, l, pcs)
    }
}
