namespace HomeTaste.Application.Authorization
{
    /// <summary>Named authorization policy constants — use with [Authorize(Policy = Policies.XYZ)].</summary>
    public static class Policies
    {
        public const string AdminOnly             = "AdminOnly";
        public const string CustomerOnly          = "CustomerOnly";
        public const string DeliveryPersonnelOnly = "DeliveryPersonnelOnly";
        public const string AdminOrDelivery       = "AdminOrDelivery";
        public const string AdminOrCustomer       = "AdminOrCustomer";
    }
}
