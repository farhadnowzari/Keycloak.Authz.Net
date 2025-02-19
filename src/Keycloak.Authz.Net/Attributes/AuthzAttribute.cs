using Microsoft.AspNetCore.Mvc;

namespace Keycloak.Authz.Net.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthzAttribute : TypeFilterAttribute
{
    /// <summary>
    /// This attribute is used to authorize the user against the Keycloak server.
    /// </summary>
    /// <remarks>
    /// If you want to leave the decision to your default policies in keycloak, use this method. It will not send any permissions to the keycloak server.
    /// </remarks>
    /// <param name="resourceFormat">The resource format is used inside the permission. Uri or Id</param>
    /// <param name="placeholderSource">Defines how the placeholders inside the permissions if any, has to be resolved.</param>
    public AuthzAttribute(PermissionResourceFormat resourceFormat = PermissionResourceFormat.Id, PlaceholderSource placeholderSource = PlaceholderSource.Params) : base(typeof(KeycloakAuthzFilter))
    {
        Arguments = [Array.Empty<string>(), resourceFormat, placeholderSource];
    }
    /// <summary>
    /// This attribute is used to authorize the user against the Keycloak server.
    /// </summary>
    /// <param name="resourceFormat">The resource format is used inside the permission. Uri or Id</param>
    /// <param name="placeholderSource">Defines how the placeholders inside the permissions if any, has to be resolved.</param>
    /// <param name="permissions">Permissions to be granted</param>
    public AuthzAttribute(string[] permissions, PermissionResourceFormat resourceFormat = PermissionResourceFormat.Id, PlaceholderSource placeholderSource = PlaceholderSource.Params) : base(typeof(KeycloakAuthzFilter))
    {
        Arguments = [permissions, resourceFormat, placeholderSource];
    }

    public string[] Permissions => (string[])Arguments[0];
    public PermissionResourceFormat ResourceFormat => (PermissionResourceFormat)Arguments[1];
    public PlaceholderSource PlaceholderSource => (PlaceholderSource)Arguments[2];
}