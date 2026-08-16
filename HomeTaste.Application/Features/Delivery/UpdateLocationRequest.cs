namespace HomeTaste.Application.Features.Delivery
{
    public record UpdateLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
