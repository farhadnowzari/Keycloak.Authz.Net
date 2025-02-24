using FS.Keycloak.RestApiClient.Model;

namespace Keycloak.Authz.Net.Resources.Contexts;

public class AuthzResourcesContext(ApiContext apiContext, AuthzScopesContext scopesContext)
{
    private string Realm => apiContext.Options.Realm;
    internal const string ResourceNameFormat = "{0}:{1}";
    public ResourcesHandler Create(string type, string identifier, string[] scopes = null, Dictionary<string, List<string>> attributes = null)
    {
        var resourceName = string.Format(ResourceNameFormat, type, identifier);

        var resourceRepresentation = new ResourceRepresentation
        {
            Attributes = attributes ?? [],
            Name = resourceName,
            DisplayName = type,
            Type = type,
            Uris = [$"/{type}"],
            Owner = new ResourceRepresentationOwner
            {
                Id = apiContext.AuthzAudienceUUID,
                Name = apiContext.Options.AuthzAudience
            }
        };

        if (scopes != null)
        {
            resourceRepresentation.Scopes = scopesContext.GetScopes(scopes);
        }

        apiContext.DefaultApi.PostClientsAuthzResourceServerResourceByClientUuid(
            Realm,
            apiContext.AuthzAudienceUUID,
            resourceRepresentation: resourceRepresentation);
        return Get(type, identifier);
    }

    internal ResourcesHandler Get(string id)
    {
        var resource = apiContext.DefaultApi.GetClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, id);
        if (resource == null)
        {
            throw new ResourceNotFoundException($"Resource {id} not found.");
        }
        return new ResourcesHandler(resource, apiContext, scopesContext);
    }

    /// <summary>
    /// Gets a resource by type and identifier.
    /// </summary>
    /// <returns>Returns ResourceHandler. Will let you to manage the queries resource.</returns>
    /// <exception cref="ResourceNotFoundException"></exception>
    public ResourcesHandler Get(string type, string identifier)
    {
        var resourceName = string.Format(ResourceNameFormat, type, identifier);
        var resources = apiContext.DefaultApi.GetClientsAuthzResourceServerResourceByClientUuid(Realm, apiContext.AuthzAudienceUUID, name: resourceName);
        if (resources.Count == 0)
        {
            throw new ResourceNotFoundException($"Resource {resourceName} not found.");
        }
        return new ResourcesHandler(resources.First(), apiContext, scopesContext);
    }
}