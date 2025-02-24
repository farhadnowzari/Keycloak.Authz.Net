using FS.Keycloak.RestApiClient.Model;

namespace Keycloak.Authz.Net.Resources.Contexts;

public class ResourcesHandler(ResourceRepresentation resource, ApiContext apiContext, AuthzScopesContext scopesContext)
{
    private string Realm => apiContext.Options.Realm;

    /// <summary>
    /// Removed the resource from the authorization server
    /// </summary>
    public void Delete()
    {
        apiContext.DefaultApi.DeleteClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id);
    }

    /// <summary>
    /// Adds a new Uri to the Uris array of the resource
    /// </summary>
    /// <param name="uri">The Uri to add to the array</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler AddUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            throw new ArgumentException("Uri cannot be null or empty.");
        }
        if (resource.Uris.Contains(uri)) return this;
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id, resourceRepresentation: resource);
        return this;
    }

    /// <summary>
    /// Removes the given Uri from the Uris array of the resource
    /// </summary>
    /// <param name="uri">The Uri to be removed</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler RemoveUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            throw new ArgumentException("Uri cannot be null or empty.");
        }
        if (!resource.Uris.Contains(uri)) return this;
        resource.Uris.Remove(uri);
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id, resourceRepresentation: resource);
        return this;
    }

    /// <summary>
    /// Adds new attributes to the resource
    /// </summary>
    /// <param name="key">The key of the attribute</param>
    /// <param name="values">The values for the attribute</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler AddAttributes(string key, List<string> values)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.");
        }
        if (values == null || values.Count == 0)
        {
            throw new ArgumentException("Values cannot be null or empty.");
        }
        if (resource.Attributes.TryGetValue(key, out List<string> value))
        {
            value.AddRange(values);
            resource.Attributes[key] = [.. value.Distinct()];
        }
        else
        {
            resource.Attributes.Add(key, values);
        }
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id, resourceRepresentation: resource);
        return this;
    }

    /// <summary>
    /// Removes the attribute from the resource
    /// </summary>
    /// <param name="key">The attribute key to be removed</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler RemoveAttributes(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.");
        }
        if (!resource.Attributes.ContainsKey(key)) return this;
        resource.Attributes.Remove(key);
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id, resourceRepresentation: resource);
        return this;
    }

    /// <summary>
    /// Removes the given values from a specific attribute
    /// </summary>
    /// <param name="key">The attribute key</param>
    /// <param name="values">The values to be removed</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler RemoveAttributes(string key, List<string> values)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.");
        }
        if (values == null || values.Count == 0)
        {
            throw new ArgumentException("Values cannot be null or empty.");
        }
        if (!resource.Attributes.TryGetValue(key, out List<string> value)) return this;
        value.RemoveAll(x => values.Contains(x));
        resource.Attributes[key] = value;
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id, resourceRepresentation: resource);
        return this;
    }

    /// <summary>
    /// Removes the given scopes from the resource
    /// </summary>
    /// <param name="scopes">Scopes to be removed</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler RemoveScopes(List<string> scopes)
    {
        if (scopes == null || scopes.Count == 0)
        {
            throw new ArgumentException("Scopes cannot be null or empty.");
        }
        resource.Scopes = [.. resource.Scopes.Where(x => !scopes.Contains(x.Name))];
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(Realm, apiContext.AuthzAudienceUUID, resource.Id, resourceRepresentation: resource);
        return this;
    }

    /// <summary>
    /// Adds the given scopes to the resource, if they are not already assigned
    /// </summary>
    /// <param name="scopes">The scopes to be added</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ResourcesHandler AddScopes(List<string> scopes)
    {
        if (scopes == null || scopes.Count == 0)
        {
            throw new ArgumentException("Scopes cannot be null or empty.");
        }
        var newScopes = scopes.Where(x => !resource.Scopes.Select(x => x.Name).Contains(x));
        resource.Scopes = [.. resource.Scopes, .. scopesContext.GetScopes(newScopes)];
        apiContext.DefaultApi.PutClientsAuthzResourceServerResourceByClientUuidAndResourceId(
            Realm,
            apiContext.AuthzAudienceUUID,
            resource.Id,
            resourceRepresentation: resource
        );
        return this;
    }
}