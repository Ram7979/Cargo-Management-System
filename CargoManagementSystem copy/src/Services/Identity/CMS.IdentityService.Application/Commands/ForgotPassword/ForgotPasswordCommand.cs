using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<ApiResponse<bool>>;
