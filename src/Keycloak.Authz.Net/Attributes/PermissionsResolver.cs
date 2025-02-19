using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Keycloak.Authz.Net.Attributes;

internal partial class PermissionsResolver(string[] permissions, HttpContext httpContext)
{
    private readonly Dictionary<string, string[]> Permissions = DetectPlaceholders(permissions);
    private readonly HttpContext HttpContext = httpContext;

    public string[] Resolve(PlaceholderSource placeholderSource)
    {
        if(Permissions.Count == 0)
        {
            return [];
        }
        if (placeholderSource == PlaceholderSource.Headers)
        {
            return [.. Permissions.Select(FromHeaders)];
        }
        if (placeholderSource == PlaceholderSource.Query)
        {
            return [.. Permissions.Select(FromQuery)];
        }
        if (placeholderSource == PlaceholderSource.Params)
        {
            return [.. Permissions.Select(FromParams)];
        }
        //Else it is PlaceholderSource.Body
        return [.. Permissions.Select(FromBody)];
    }
    private string FromHeaders(KeyValuePair<string, string[]> input)
    {
        var placeholders = input.Value;
        var permission = input.Key;
        foreach (var placeholder in placeholders)
        {
            if (!HttpContext.Request.Headers.TryGetValue(placeholder, out StringValues value))
            {
                throw new ArgumentException($"The placeholder '{placeholder}' could not be resolved from the headers.");
            }
            permission = permission.Replace("{" + placeholder + "}", value);
        }
        return permission;

    }
    private string FromQuery(KeyValuePair<string, string[]> input)
    {
        var placeholders = input.Value;
        var permission = input.Key;
        foreach (var placeholder in placeholders)
        {
            if (!HttpContext.Request.Query.TryGetValue(placeholder, out var value))
            {
                throw new ArgumentException($"The placeholder '{placeholder}' could not be resolved from the query.");
            }
            permission = permission.Replace("{" + placeholder + "}", value);
        }
        return permission;
    }


    private string FromParams(KeyValuePair<string, string[]> input)
    {
        var placeholders = input.Value;
        var permission = input.Key;
        foreach (var placeholder in placeholders)
        {
            var value = HttpContext.GetRouteValue(placeholder) ?? throw new ArgumentException($"The placeholder '{placeholder}' could not be resolved from the route parameters.");
            permission = permission.Replace("{" + placeholder + "}", value.ToString());
        }
        return permission;
    }

    private string FromBody(KeyValuePair<string, string[]> input)
    {
        var placeholders = input.Value;
        var permission = input.Key;
        //So the body is readable again later
        HttpContext.Request.EnableBuffering();
        using var reader = new StreamReader(HttpContext.Request.Body, encoding: Encoding.UTF8, leaveOpen: true);
        var bodyString = reader.ReadToEnd();
        var body = JsonSerializer.Deserialize<Dictionary<string, JsonValue>>(bodyString);
        foreach (var placeholder in placeholders)
        {
            
            if (!body.TryGetValue(placeholder, out var value))
            {
                throw new ArgumentException($"The placeholder '{placeholder}' could not be resolved from the body.");
            }
            permission = permission.Replace("{" + placeholder + "}", value.ToString());
        }
        //Reset the position to 0
        HttpContext.Request.Body.Position = 0;
        return permission;
    }

    public static Dictionary<string, string[]> DetectPlaceholders(string[] input)
    {
        return input.Select(x =>
        {
            var matches = PlaceholderRegex().Matches(x);
            if (matches.Count == 0)
            {
                return new KeyValuePair<string, string[]>(x, []);
            }
            KeyValuePair<string, string[]> result = new(x, [.. matches.Select(y => y.Groups[1].Value)]);
            return result;
        }).ToDictionary(x => x.Key, x => x.Value);
    }

    [GeneratedRegex(@"{(\w+)}")]
    private static partial Regex PlaceholderRegex();
}