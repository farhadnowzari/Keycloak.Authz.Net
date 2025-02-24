using Keycloak.Authz.Net.Resources.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Authz.Net.Resources;

public static class DI
{
    public static IServiceCollection AddAuthzResources(this IServiceCollection services)
    {
        services.AddScoped<ApiContext>();
        services.AddScoped<AuthzResourcesContext>();
        services.AddScoped<AuthzScopesContext>();
        return services;
    }
}