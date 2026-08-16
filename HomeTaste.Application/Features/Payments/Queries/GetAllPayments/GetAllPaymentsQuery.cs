using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Queries.GetAllPayments
{
    public record GetAllPaymentsQuery(int PageNumber = 1, int PageSize = 10, PaymentStatus? Status = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>>;
}
