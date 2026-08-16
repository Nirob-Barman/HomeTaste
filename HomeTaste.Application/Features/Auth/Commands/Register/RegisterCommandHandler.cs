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
            //if (!_userContextService.IsAuthenticated)
            //{
            //    throw new UnauthorizedException("You must be logged in as an Admin to register new users.");
            //}

            //if (!_userContextService.IsInRole("Admin"))
            //{
            //    throw new UnauthorizedException("Only Admins can register new users.");
            //}

            if (command.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedException("You are not allowed to assign the 'Admin' role.");
            }

            var existingUser = await _userManager.FindByEmailAsync(command.Email!);
            if (existingUser != null)
                throw new ConflictException("A user with this email already exists.");

            var user = new ApplicationUser
            {
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                DateOfBirth = command.DateOfBirth,
            };

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var (success, userId, errors) = await _userManager.CreateAsync(user, command.Password!);
            if (!success)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BadRequestException($"User creation failed: {string.Join(", ", errors)}");
            }

            if (!string.IsNullOrWhiteSpace(command.Role))
            {
                var (roleAssignSuccess, roleErrors) = await _userManager.AddToRoleAsync(user, command.Role!);
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
                Email = command.Email!,
                Role = command.Role
            };

            return Result<RegisterResponse>.Ok(registerResponse, "User registered successfully");
        }
    }
}
