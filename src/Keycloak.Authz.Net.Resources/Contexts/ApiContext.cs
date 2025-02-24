using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.Keycloak.RestApiClient.Authentication.ClientFactory;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.Keycloak.RestApiClient.ClientFactory;
using Microsoft.Extensions.Options;

namespace Keycloak.Authz.Net.Resources.Contexts;

public class ApiContext
{
    private AuthenticationHttpClient HttpClient { get; }
    internal DefaultApi DefaultApi { get; }
    internal ClientsApi ClientsApi { get; }
    public string AuthzAudienceUUID { get; }
    internal AuthzOptions Options { get; }

    public ApiContext(IOptions<AuthzOptions> options)
    {
        Options = options.Value;
        var credentials = new ClientCredentialsFlow
        {
            ClientId = options.Value.ClientId,
            ClientSecret = options.Value.ClientSecret,
            KeycloakUrl = options.Value.BaseUrl,
            Realm = options.Value.Realm
        };
        HttpClient = AuthenticationHttpClientFactory.Create(credentials);
        DefaultApi = ApiClientFactory.Create<DefaultApi>(HttpClient);
        ClientsApi = ApiClientFactory.Create<ClientsApi>(HttpClient);
        AuthzAudienceUUID = GetClientUUID();
        if (string.IsNullOrEmpty(AuthzAudienceUUID))
        {
            throw new ConfigurationException("AuthzAudienceUUID could not be fetched nor be found in the configuration.");
        }
    }
    private string GetClientUUID()
    {
        if (!string.IsNullOrEmpty(Options.AuthzAudienceUUID))
        {
            return Options.AuthzAudienceUUID;
        }
        var clients = ClientsApi.GetClients(Options.Realm, Options.AuthzAudience);
        return clients.FirstOrDefault()?.Id;
    }
}