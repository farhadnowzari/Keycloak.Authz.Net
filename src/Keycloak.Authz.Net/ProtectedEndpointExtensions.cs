using Keycloak.Authz.Net.Attributes;
using Microsoft.AspNetCore.Builder;

namespace Keycloak.Authz.Net;

public static class ProtectedEndpointExtensions
{
    /// <summary>
    /// This method protects the endpoint by adding the AuthzAttribute to the endpoint metadata.
    /// </summary>
    public static TBuilder RequireAuthz<TBuilder>(this TBuilder endpoint, string[] permissions, PermissionResourceFormat permissionResourceFormat = PermissionResourceFormat.Id, PlaceholderSource placeholderSource = PlaceholderSource.Params) where TBuilder : IEndpointConventionBuilder
    {
        endpoint.Add(endpointBuilder =>
        {
            if (!endpointBuilder.Metadata.Any(meta => meta is AuthzAttribute))
            {
                endpointBuilder.Metadata.Add(new AuthzAttribute(permissions, permissionResourceFormat, placeholderSource));
            }
        });
        return endpoint;
    }

    public static TBuilder RequireAuthz<TBuilder>(this TBuilder endpoint, PermissionResourceFormat permissionResourceFormat = PermissionResourceFormat.Id, PlaceholderSource placeholderSource = PlaceholderSource.Params) where TBuilder : IEndpointConventionBuilder
    {
        endpoint.Add(endpointBuilder =>
        {
            if (!endpointBuilder.Metadata.Any(meta => meta is AuthzAttribute))
            {
                endpointBuilder.Metadata.Add(new AuthzAttribute(permissionResourceFormat, placeholderSource));
            }
        });
        return endpoint;
    }
}