using Keycloak.Authz.Net.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Authz.Net;

public class AuthzMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if(endpoint != null) {
            var authzMetadata = endpoint.Metadata.GetMetadata<AuthzAttribute>();
            if(authzMetadata != null) {
                var authzContext = context.RequestServices.GetRequiredService<AuthzContext>();
                var resolvedPermissions = new PermissionsResolver(authzMetadata.Permissions, context).Resolve(authzMetadata.PlaceholderSource);
                var result = await authzContext.AuthorizeAsync(resolvedPermissions, authzMetadata.ResourceFormat, context.RequestAborted);
                if(!result) {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("{ \"message\": \"Unauthorized\" }");
                    return;
                }
            }
        }
        await next(context);
    }
}