using FluentValidation;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateDeliveryPersonnel
{
    public class UpdateDeliveryPersonnelCommandValidator : AbstractValidator<UpdateDeliveryPersonnelCommand>
    {
        public UpdateDeliveryPersonnelCommandValidator()
        {
            RuleFor(x => x.FullName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Full name is required.")
                .Must(v => v!.Trim().Length <= 150).WithMessage("Full name cannot exceed 150 characters.");

            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Phone number is required.")
                .Must(v => v!.Trim().Length <= 20).WithMessage("Phone number cannot exceed 20 characters.");

            RuleFor(x => x.VehicleType)
                .MaximumLength(50).WithMessage("Vehicle type cannot exceed 50 characters.");

            RuleFor(x => x.VehicleNumber)
                .MaximumLength(50).WithMessage("Vehicle number cannot exceed 50 characters.");
        }
    }
}
