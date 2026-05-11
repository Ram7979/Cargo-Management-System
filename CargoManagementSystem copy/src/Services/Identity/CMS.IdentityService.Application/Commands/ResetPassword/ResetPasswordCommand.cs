using CMS.Shared.Responses;
using MediatR;

namespace CMS.IdentityService.Application.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<ApiResponse<bool>>;
