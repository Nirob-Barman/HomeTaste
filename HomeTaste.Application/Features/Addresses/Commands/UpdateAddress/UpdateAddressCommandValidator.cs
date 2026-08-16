using FluentValidation;

namespace HomeTaste.Application.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressCommandValidator()
        {
            RuleFor(x => x.AddressLine1)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Address line 1 is required.")
                .Must(v => v!.Trim().Length <= 200).WithMessage("Address line 1 cannot exceed 200 characters.");

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200).WithMessage("Address line 2 cannot exceed 200 characters.");

            RuleFor(x => x.City)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("City is required.")
                .Must(v => v!.Trim().Length <= 100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.State)
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Country is required.")
                .Must(v => v!.Trim().Length <= 100).WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
                .When(x => x.Latitude.HasValue);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
                .When(x => x.Longitude.HasValue);

            RuleFor(x => x.Label)
                .MaximumLength(50).WithMessage("Label cannot exceed 50 characters.");
        }
    }
}
