using FluentValidation;

namespace HomeTaste.Application.Features.Addresses.Commands.CreateAddress
{
    public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
    {
        public CreateAddressCommandValidator()
        {
            RuleFor(x => x.Request.AddressLine1)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Address line 1 is required.")
                .Must(v => v!.Trim().Length <= 200).WithMessage("Address line 1 cannot exceed 200 characters.");

            RuleFor(x => x.Request.AddressLine2)
                .MaximumLength(200).WithMessage("Address line 2 cannot exceed 200 characters.");

            RuleFor(x => x.Request.City)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("City is required.")
                .Must(v => v!.Trim().Length <= 100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Request.State)
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            RuleFor(x => x.Request.Country)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Country is required.")
                .Must(v => v!.Trim().Length <= 100).WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.Request.PostalCode)
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");

            RuleFor(x => x.Request.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
                .When(x => x.Request.Latitude.HasValue);

            RuleFor(x => x.Request.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
                .When(x => x.Request.Longitude.HasValue);

            RuleFor(x => x.Request.Label)
                .MaximumLength(50).WithMessage("Label cannot exceed 50 characters.");
        }
    }
}
