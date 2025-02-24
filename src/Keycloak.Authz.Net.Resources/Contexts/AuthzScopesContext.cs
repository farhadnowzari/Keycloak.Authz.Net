using FS.Keycloak.RestApiClient.Model;
using Microsoft.Extensions.Logging;

namespace Keycloak.Authz.Net.Resources.Contexts;


public class AuthzScopesContext(ApiContext apiContext)
{
    private string Realm => apiContext.Options.Realm;
    public void Create(string name, string description = null)
    {
        var scopeRepresentation = new ScopeRepresentation
        {
            Name = name,
        };
        apiContext.DefaultApi.PostClientsAuthzResourceServerScopeByClientUuid(Realm, apiContext.AuthzAudienceUUID, scopeRepresentation);
    }

    public List<ScopeRepresentation> GetScopes(IEnumerable<string> names)
    {
        var scopes = apiContext.DefaultApi.GetClientsAuthzResourceServerScopeByClientUuid(Realm, apiContext.AuthzAudienceUUID);
        return [.. scopes.Where(x => names.Contains(x.Name))];
    }
}