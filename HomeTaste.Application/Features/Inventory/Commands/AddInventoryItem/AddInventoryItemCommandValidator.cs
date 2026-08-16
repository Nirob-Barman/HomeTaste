using FluentValidation;

namespace HomeTaste.Application.Features.Inventory.Commands.AddInventoryItem
{
    public class AddInventoryItemCommandValidator : AbstractValidator<AddInventoryItemCommand>
    {
        public AddInventoryItemCommandValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Item name is required.")
                .Must(name => name!.Trim().Length <= 200).WithMessage("Item name cannot exceed 200 characters.");

            RuleFor(x => x.StockCount)
                .GreaterThanOrEqualTo(0).WithMessage("Stock count cannot be negative.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
        }
    }
}
