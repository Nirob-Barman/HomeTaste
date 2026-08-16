using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using LoyaltyTransactionEntity = HomeTaste.Domain.Entities.Loyalty.LoyaltyTransaction;

namespace HomeTaste.Application.Features.Loyalty.Commands.AdjustPoints
{
    public class AdjustPointsCommandHandler : IRequestHandler<AdjustPointsCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public AdjustPointsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(AdjustPointsCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var account = await LoyaltyAccountHelper.GetOrCreateAccountAsync(_context, request.UserId!, cancellationToken);

            if (request.Points < 0 && account.CurrentPoints + request.Points < 0)
                throw new BadRequestException("Adjustment would result in a negative balance.");

            account.AdjustPoints(request.Points);

            var transaction = LoyaltyTransactionEntity.Create(
                account.Id,
                request.Points,
                LoyaltyTransactionType.Adjusted,
                null,
                request.Description ?? "Admin adjustment");

            _context.LoyaltyTransactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, $"Points adjusted by {request.Points:+#;-#;0}.");
        }
    }
}
