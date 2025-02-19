using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Keycloak.Authz.Net.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Keycloak.Authz.Net;

public class KeycloakAuthzContext(IOptions<AuthzOptions> authzOptions, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<KeycloakAuthzContext> logger)
{
    internal const string HttpClientKey = "keycloak-authz";
    internal const string GrantType = "urn:ietf:params:oauth:grant-type:uma-ticket";
    internal HttpClient HttpClient = httpClientFactory.CreateClient(HttpClientKey);
    internal HttpContext HttpContext => httpContextAccessor.HttpContext;
    /// <summary>
    /// If you want to leave the decision to your default policies in keycloak, use this method. It will not send any permissions to the keycloak server.
    /// </summary>
    public async Task<bool> AuthorizeAsync(PermissionResourceFormat? resourceFormat, CancellationToken cancellationToken = default)
    {
        return await AuthorizeAsync([], resourceFormat, cancellationToken);
    }

    /// <summary>
    /// Authorizes the permissions against the Keycloak server.
    /// </summary>
    /// <param name="permissions">permissions in the given format from keycloak. e.g. grade, grade_1#create, #read. Check <see href="https://www.keycloak.org/docs/latest/authorization_services/index.html#_service_obtaining_permissions">Keycloak Permission API</see></param>
    /// <param name="resourceFormat">This is how resource part of the permission is formatted. Do you expect to authorize by the Uri of the resource or it's Name/Id. The default value is Id</param>
    /// <returns>True if the permission is granted, False if there is something wrong. The exceptions will be logged.</returns>
    public async Task<bool> AuthorizeAsync(string[] permissions, PermissionResourceFormat? resourceFormat, CancellationToken cancellationToken = default)
    {
        var content = BuildRequestBody(decision: true, permissions: permissions, resourceFormat: resourceFormat);
        logger.LogDebug("Requesting authorization from Keycloak server. Permission(s): {Permissions}", permissions);
        var response = await HttpClient.PostAsync(authzOptions.Value.TokenEndpoint, content, cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<KeycloakSuccessfulResponse>(json);
            logger.LogDebug("Authorization result for permission(s) {Permissions}: {Result}", permissions, result?.Result ?? false);
            return result?.Result ?? false;
        }
        catch (HttpRequestException ex)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Failed to authorize permission(s) {Permissions}.", permissions);
            logger.LogDebug("Keycloak response body on failure: {json}", json);
            logger.LogError(ex, "Failed to authorize permission(s) {Permissions}.", permissions);
            return false;
        }
    }


    /// <summary>
    /// This method is used to get all the permissions list from the Keycloak server. That the user can do defined actions to.
    /// </summary>
    public async Task<IEnumerable<KeycloakPermission>> GetPermissionsAsync(PermissionResourceFormat? resourceFormat, CancellationToken cancellationToken = default)
    {
        return await GetPermissionsAsync([], resourceFormat, cancellationToken);
    }

    /// <summary>
    /// This method is used to get the permissions list from the Keycloak server.
    /// </summary>
    /// <remarks>
    /// This method is useful when you want to get list of all resources with specific actions/scopes. e.g. query with permissions.
    /// </remarks>
    /// <param name="permissions">permissions in the given format from keycloak. e.g. grade, grade_1#create, #read. Check <see href="https://www.keycloak.org/docs/latest/authorization_services/index.html#_service_obtaining_permissions">Keycloak Permission API</see></param>
    /// <param name="resourceFormat">This is how resource part of the permission is formatted. Do you expect to authorize by the Uri of the resource or it's Name/Id. The default value is Id</param>
    /// <returns>Returns a list of permissions with the possible actions on them</returns>
    public async Task<IEnumerable<KeycloakPermission>> GetPermissionsAsync(string[] permissions, PermissionResourceFormat? resourceFormat, CancellationToken cancellationToken = default)
    {
        var content = BuildRequestBody(decision: false, permissions: permissions, resourceFormat: resourceFormat);
        logger.LogDebug("Requesting permissions from Keycloak server. Permission(s): {Permissions}", permissions);
        var response = await HttpClient.PostAsync(authzOptions.Value.TokenEndpoint, content, cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<List<KeycloakPermission>>(json);
            logger.LogDebug("Permissions for permission(s) {Permissions}: {Result}", permissions, result);
            return result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning("Failed to get permissions for permission(s) {Permissions}.", permissions);
            logger.LogError(ex, "Failed to get permissions for permission(s) {Permissions}.", permissions);
            return [];
        }
    }

    private FormUrlEncodedContent BuildRequestBody(bool decision, string[] permissions, PermissionResourceFormat? resourceFormat)
    {
        var body = new Dictionary<string, string>
        {
            ["grant_type"] = GrantType,
            ["audience"] = authzOptions.Value.AuthzAudience
        };
        if (permissions.Length > 0)
        {
            body["permission"] = string.Join(",", permissions);
        }
        if (decision)
        {
            body["response_mode"] = "decision";
        }
        else
        {
            body["response_mode"] = "permissions";
        }
        if (resourceFormat.HasValue)
        {
            body["permission_resource_format"] = resourceFormat.Value.ToString().ToLowerInvariant();
        }
        var content = new FormUrlEncodedContent(body);
        if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeader.ToString().Split(' ')[1]);
        }
        else
        {
            logger.LogWarning("Authorization header is not found in the request. The request may be unauthorized.");
        }
        return content;

    }
}

public record KeycloakSuccessfulResponse
{
    [JsonPropertyName("result")]
    public bool Result { get; init; }
}

public record KeycloakPermission
{
    /// <summary>
    /// These are the actions/scopes can be done on the resource.
    /// </summary>
    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }
    /// <summary>
    /// This the keycloak's resource id.
    [JsonPropertyName("rsid")]
    public string ResourceId { get; set; }
    /// <summary>
    /// This is the unique name or id of your resource, synced with keycloak.
    /// </summary>
    [JsonPropertyName("rsname")]
    public string ResourceName { get; set; }
}