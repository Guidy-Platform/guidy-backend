using CoursePlatform.Application.Features.Payouts.DTOs;
using MediatR;

namespace CoursePlatform.Application.Features.Payouts.Queries.GetMyWallet;

public record GetMyWalletQuery : IRequest<WalletDto>;