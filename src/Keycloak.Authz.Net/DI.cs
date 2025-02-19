using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Authz.Net;

public static class DI
{
    /// <summary>
    /// Configures authentication and authorization for keycloak
    /// </summary>
    /// <remarks>
    /// Verifying the token will be already approved by 
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddKeycloakAuthz(this IServiceCollection services, Action<AuthzOptions> configure)
    {
        services.Configure(configure);
        var options = new AuthzOptions();
        configure(options);
        services.AddHttpClient(KeycloakAuthzContext.HttpClientKey, config =>
        {
            config.BaseAddress = new Uri(options.BaseUrl);
        });
        services.AddScoped<KeycloakAuthzContext>();
        return services;
    }

    /// <summary>
    /// Registers Authz middleware for keycloak. This is needed only if using the RequiredAuthz extension method on endpoints.
    /// </summary>
    /// <returns></returns>
    public static IApplicationBuilder UseKeycloakAuthz(this IApplicationBuilder app)
    {
        app.UseMiddleware<AuthzMiddleware>();
        return app;
    }
}
