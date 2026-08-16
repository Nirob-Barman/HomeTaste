using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(string? FirstName, string? LastName, DateTime? DateOfBirth, string? Email, string? Password, string Role = "Customer")
        : IRequest<Result<RegisterResponse>>;
}
