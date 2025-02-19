using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Keycloak.Authz.Net.Attributes;

public class KeycloakAuthzFilter : IAsyncAuthorizationFilter
{
    private string[] Permissions { get; }
    private KeycloakAuthzContext AuthzContext { get; }
    public PermissionResourceFormat? ResourceFormat { get; }

    // Don't use the primary constructor here. It can cause misunderstanding on which permissions array should be used and can cause bugs.
    public KeycloakAuthzFilter(KeycloakAuthzContext kcAuthzContext, string[] permissions, PermissionResourceFormat? resourceFormat = null, PlaceholderSource placeholderSource = PlaceholderSource.Params)
    {
        Permissions = new PermissionsResolver(permissions, kcAuthzContext.HttpContext).Resolve(placeholderSource) ?? [];
        AuthzContext = kcAuthzContext;
        ResourceFormat = resourceFormat;
    }


    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!await AuthzContext.AuthorizeAsync(Permissions, resourceFormat: ResourceFormat, context.HttpContext.RequestAborted))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}