using HomeTaste.Application.DTOs.Payment;

namespace HomeTaste.Application.Validators.Payment
{
    public static class ConfirmPaymentRequestValidator
    {
        public static List<string> Validate(ConfirmPaymentRequest request)
        {
            var errors = new List<string>();

            if (request.TransactionRef?.Length > 200)
                errors.Add("Transaction reference cannot exceed 200 characters.");

            if (request.Notes?.Length > 500)
                errors.Add("Notes cannot exceed 500 characters.");

            return errors;
        }
    }
}
