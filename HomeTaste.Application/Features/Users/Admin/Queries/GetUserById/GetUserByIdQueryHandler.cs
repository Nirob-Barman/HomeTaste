using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<AdminUserResponse>>
    {
        private readonly IUserManager _userManager;

        public GetUserByIdQueryHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<AdminUserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            return Result<AdminUserResponse>.Ok(new AdminUserResponse
            {
                Id = user.Id!,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ProfileImageUrl = user.ProfileImageUrl,
                IsLocked = user.IsLocked,
                Roles = roles.ToList()
            }, "User retrieved successfully");
        }
    }
}
