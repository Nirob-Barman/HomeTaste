using HomeTaste.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Addresses
{
    public static class AddressDefaultHelper
    {
        public static async Task ClearDefaultFlagAsync(IApplicationDbContext context, Guid userId, CancellationToken cancellationToken)
        {
            var existingDefaults = await context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingDefaults)
            {
                existing.ClearDefault();
            }
        }
    }
}
