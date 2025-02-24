using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Authz.Net;

public class KeycloakAuthzDependencyChain(IServiceCollection services, AuthzOptions options)
{
    public IServiceCollection Services => services;
    public AuthzOptions Options => options;
}