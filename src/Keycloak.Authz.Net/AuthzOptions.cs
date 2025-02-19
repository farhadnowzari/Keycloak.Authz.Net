using Keycloak.Authz.Net.Attributes;

namespace Keycloak.Authz.Net;

public record AuthzOptions
{
    /// <summary>
    /// This is the base URL of the Keycloak server.
    /// </summary>
    public string BaseUrl { get; set; }
    /// <summary>
    /// The keycloak realm name where your users are coming from.
    /// </summary>
    public string Realm { get; set; }
    /// <summary>
    /// The client ID which owns the authorization.
    /// </summary>
    public string AuthzAudience { get; set; }

    internal string TokenEndpoint => $"/realms/{Realm}/protocol/openid-connect/token";
    internal string JwksEndpoint => $"/realms/{Realm}/protocol/openid-connect/certs";
}