using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.GoogleSso;

public record GoogleSsoCommand(string AuthorizationCode) : IRequest<Result<GoogleSsoResponse>>;