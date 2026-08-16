using FluentValidation;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.CreateDeliveryPersonnel
{
    public class CreateDeliveryPersonnelCommandValidator : AbstractValidator<CreateDeliveryPersonnelCommand>
    {
        public CreateDeliveryPersonnelCommandValidator()
        {
            RuleFor(x => x.Request.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Full name is required.")
                .Must(v => v!.Trim().Length <= 150).WithMessage("Full name cannot exceed 150 characters.");

            RuleFor(x => x.Request.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Phone number is required.")
                .Must(v => v!.Trim().Length <= 20).WithMessage("Phone number cannot exceed 20 characters.");

            RuleFor(x => x.Request.VehicleType)
                .MaximumLength(50).WithMessage("Vehicle type cannot exceed 50 characters.");

            RuleFor(x => x.Request.VehicleNumber)
                .MaximumLength(50).WithMessage("Vehicle number cannot exceed 50 characters.");
        }
    }
}
