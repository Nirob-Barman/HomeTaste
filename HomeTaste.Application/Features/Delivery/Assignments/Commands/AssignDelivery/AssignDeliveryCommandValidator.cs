using FluentValidation;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.AssignDelivery
{
    public class AssignDeliveryCommandValidator : AbstractValidator<AssignDeliveryCommand>
    {
        public AssignDeliveryCommandValidator()
        {
            RuleFor(x => x.Request.OrderId).NotEqual(Guid.Empty).WithMessage("OrderId is required.");
            RuleFor(x => x.Request.DeliveryPersonnelId).NotEqual(Guid.Empty).WithMessage("DeliveryPersonnelId is required.");
        }
    }
}
