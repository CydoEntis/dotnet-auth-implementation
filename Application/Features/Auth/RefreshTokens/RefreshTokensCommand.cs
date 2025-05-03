using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.RefreshTokens;

public record RefreshTokensCommand : IRequest<Result<RefreshTokensResponse>>;