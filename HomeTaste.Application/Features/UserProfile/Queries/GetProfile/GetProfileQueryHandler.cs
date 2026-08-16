using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserProfileResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;

        public GetProfileQueryHandler(IUserManager userManager, IUserContextService userContextService)
        {
            _userManager = userManager;
            _userContextService = userContextService;
        }

        public async Task<Result<UserProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Unauthorized");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

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
            }, "Profile retrieved successfully");
        }
    }
}
