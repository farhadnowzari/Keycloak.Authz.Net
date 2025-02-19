namespace Keycloak.Authz.Net.Attributes;

public enum PermissionResourceFormat {
    /// <summary>
    /// The permission resource format is the id of the resource (a.k.a resource_name in keycloak)
    /// </summary>
    Id,
    /// <summary>
    /// The permission resource format is the uri of the resource
    /// </summary>
    Uri
}