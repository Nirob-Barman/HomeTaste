using HomeTaste.Application.DTOs.Payment;

namespace HomeTaste.Application.Validators.Payment
{
    public static class ConfirmDirectPaymentRequestValidator
    {
        public static List<string> Validate(ConfirmDirectPaymentRequest request)
        {
            var errors = new List<string>();

            if (request.OrderId == Guid.Empty)
                errors.Add("Order ID is required.");

            if (string.IsNullOrWhiteSpace(request.Gateway))
                errors.Add("Gateway is required.");

            if (request.TransactionRef?.Length > 200)
                errors.Add("Transaction reference cannot exceed 200 characters.");

            if (request.Notes?.Length > 500)
                errors.Add("Notes cannot exceed 500 characters.");

            return errors;
        }
    }
}
