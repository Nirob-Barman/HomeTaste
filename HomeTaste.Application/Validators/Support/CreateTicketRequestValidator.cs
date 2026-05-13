using HomeTaste.Application.DTOs.Support;
using HomeTaste.Domain.Entities.Support;

namespace HomeTaste.Application.Validators.Support
{
    public static class CreateTicketRequestValidator
    {
        public static List<string> Validate(CreateTicketRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Subject))
                errors.Add("Subject is required.");
            else if (request.Subject.Trim().Length > 200)
                errors.Add("Subject cannot exceed 200 characters.");

            if (string.IsNullOrWhiteSpace(request.Description))
                errors.Add("Description is required.");
            else if (request.Description.Trim().Length > 2000)
                errors.Add("Description cannot exceed 2000 characters.");

            if (!Enum.IsDefined(typeof(TicketPriority), request.Priority))
                errors.Add("Invalid ticket priority.");

            if (request.MobileNo != null && request.MobileNo.Trim().Length > 20)
                errors.Add("Mobile number cannot exceed 20 characters.");

            return errors;
        }
    }
}
