namespace Keycloak.Authz.Net.Resources;

public class ResourceNotFoundException(string name): Exception($"Resource {name} not found.");