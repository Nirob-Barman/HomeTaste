using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionById
{
    public record GetOptionByIdQuery(Guid Id) : IRequest<Result<MealCustomizationOptionResponse>>;
}
