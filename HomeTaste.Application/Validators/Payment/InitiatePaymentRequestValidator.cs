using HomeTaste.Application.DTOs.Payment;

namespace HomeTaste.Application.Validators.Payment
{
    public static class InitiatePaymentRequestValidator
    {
        public static List<string> Validate(InitiatePaymentRequest request)
        {
            var errors = new List<string>();

            if (request.OrderId == Guid.Empty)
                errors.Add("OrderId is required.");

            if (request.Gateway != null && request.Gateway.Trim().Length > 50)
                errors.Add("Gateway name cannot exceed 50 characters.");

            if (request.Notes?.Length > 500)
                errors.Add("Notes cannot exceed 500 characters.");

            return errors;
        }
    }
}
