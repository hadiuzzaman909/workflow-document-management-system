using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Enums;


public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAdminPermissionService _adminPermissionService;

    public PermissionHandler(IAdminPermissionService adminPermissionService)
    {
        _adminPermissionService = adminPermissionService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            context.Fail();
            return;
        }

        bool hasPermission = await _adminPermissionService.AdminHasPermissionAsync(userId, requirement.RequiredAccessLevel);

        if (hasPermission)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}
