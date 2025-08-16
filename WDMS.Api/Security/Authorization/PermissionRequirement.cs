using Microsoft.AspNetCore.Authorization;
using WDMS.Domain.Enums;

public class PermissionRequirement : IAuthorizationRequirement
{
    public AccessLevel RequiredAccessLevel { get; private set; }

    public PermissionRequirement(AccessLevel requiredAccessLevel)
    {
        RequiredAccessLevel = requiredAccessLevel;
    }
}