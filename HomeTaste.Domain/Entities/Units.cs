namespace HomeTaste.Domain.Entities
{
    public class Units : BaseEntity
    {
        public string? Name { get; private set; }           // Name of the Unit (e.g., Kilogram, Gram, Liter, Piece)
        public string? Abbreviation { get; private set; }   // Abbreviation for the Unit (e.g., kg, g, l, pcs)

        private Units() { } // EF Core

        public static Units Create(string? name, string? abbreviation)
        {
            return new Units
            {
                Name = name,
                Abbreviation = abbreviation
            };
        }

        public void UpdateDetails(string? name, string? abbreviation)
        {
            Name = name ?? Name;
            Abbreviation = abbreviation ?? Abbreviation;
        }
    }
}
