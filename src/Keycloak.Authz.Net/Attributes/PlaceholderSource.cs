namespace Keycloak.Authz.Net.Attributes;

public enum PlaceholderSource
{
    /// <summary>
    /// Will resolve the placeholders from request headers
    /// </summary>
    Headers,
    /// <summary>
    /// Will resolve the placeholders from request query string
    /// </summary>
    Query,
    /// <summary>
    /// Will resolve the placeholders from request route parameters
    /// </summary>
    Params,
    /// <summary>
    /// Will resolve the placeholders from request body
    /// </summary>
    Body
}