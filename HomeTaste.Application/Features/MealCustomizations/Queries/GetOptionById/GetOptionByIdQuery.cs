using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCustomizations.Queries.GetOptionById
{
    public class GetOptionByIdQuery : IRequest<Result<MealCustomizationOptionResponse>>
    {
        public Guid Id { get; set; }

        public GetOptionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
