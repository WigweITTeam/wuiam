using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WUIAM.Enums;
using WUIAM.Interfaces;
namespace WUIAM.Attributes
{

}
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly Permissions _permission;

    public HasPermissionAttribute(Permissions permission)
    {
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var userId = userIdClaim.Value;
        if (!Guid.TryParse(userId, out Guid parsedUserId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissionService = context.HttpContext.RequestServices
            .GetRequiredService<IPermissionService>();

        var hasPermission = await permissionService.UserHasPermissionAsync(parsedUserId, _permission.ToString());

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}
