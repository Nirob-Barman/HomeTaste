using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly IApplicationDbContext _context;

        public RegisterCommandHandler(IUserManager userManager, IApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            //if (!_userContextService.IsAuthenticated)
            //{
            //    throw new UnauthorizedException("You must be logged in as an Admin to register new users.");
            //}

            //if (!_userContextService.IsInRole("Admin"))
            //{
            //    throw new UnauthorizedException("Only Admins can register new users.");
            //}

            if (request.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedException("You are not allowed to assign the 'Admin' role.");
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email!);
            if (existingUser != null)
                throw new ConflictException("A user with this email already exists.");

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
            };

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var (success, userId, errors) = await _userManager.CreateAsync(user, request.Password!);
            if (!success)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BadRequestException($"User creation failed: {string.Join(", ", errors)}");
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var (roleAssignSuccess, roleErrors) = await _userManager.AddToRoleAsync(user, request.Role!);
                if (!roleAssignSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ServerErrorException($"Role assignment failed: {string.Join(", ", roleErrors)}");
                }
            }

            await transaction.CommitAsync(cancellationToken);

            var registerResponse = new RegisterResponse
            {
                Id = userId!,
                Email = request.Email!,
                Role = request.Role
            };

            return Result<RegisterResponse>.Ok(registerResponse, "User registered successfully");
        }
    }
}
