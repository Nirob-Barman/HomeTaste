using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Queries.GetUnitById
{
    public class GetUnitByIdQuery : IRequest<Result<UnitResponse>>
    {
        public Guid Id { get; set; }

        public GetUnitByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
