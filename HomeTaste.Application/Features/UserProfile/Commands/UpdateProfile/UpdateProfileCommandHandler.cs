using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UserProfileResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;

        public UpdateProfileCommandHandler(IUserManager userManager, IUserContextService userContextService)
        {
            _userManager = userManager;
            _userContextService = userContextService;
        }

        public async Task<Result<UserProfileResponse>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Unauthorized");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;

            var (succeeded, errors) = await _userManager.UpdateAsync(user);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            var roles = await _userManager.GetRolesAsync(user);

            return Result<UserProfileResponse>.Ok(new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                Roles = roles.ToList()
            }, "Profile updated successfully");
        }
    }
}
