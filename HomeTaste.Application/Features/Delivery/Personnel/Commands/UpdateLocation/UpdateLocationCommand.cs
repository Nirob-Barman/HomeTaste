using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateLocation
{
    public class UpdateLocationCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public UpdateLocationRequest Request { get; set; }

        public UpdateLocationCommand(Guid id, UpdateLocationRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
