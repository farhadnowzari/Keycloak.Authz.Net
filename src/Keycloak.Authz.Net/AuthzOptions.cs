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
    /// <summary>
    /// The UUID of the client ID which owns the authorization.
    /// </summary>
    /// <remarks>
    /// If this is not set and you are using the other packages from Keycloak.Authz.Net, this will be fetched automatically every time it is needed.
    /// </remarks>
    public string AuthzAudienceUUID { get; set; }
    private string _clientId;
    /// <summary>
    /// The ClientId you want to use to let the service to manage your keycloak realm.
    /// </summary>
    /// <remarks>
    /// This is only needed if you are using other dependency packages from Keycloak.Authz.Net
    /// </remarks>
    /// <value>By default is AuthzAudience if not set</value>
    public string ClientId
    {
        get => _clientId ?? AuthzAudience;
        set => _clientId = value;
    }

    /// <summary>
    /// The client secret of the client ID. If the ClientId is not set, this means you need to add the secret to the AuthzAudience.
    /// </summary>
    /// <remarks>
    /// This is only needed if you are using other dependency packages from Keycloak.Authz.Net
    /// </remarks>
    public string ClientSecret { get; set; }
    internal string TokenEndpoint => $"/realms/{Realm}/protocol/openid-connect/token";
    internal string JwksEndpoint => $"/realms/{Realm}/protocol/openid-connect/certs";
}