namespace Keycloak.Authz.Net.Tests.Attributes;

using FluentAssertions;
using Keycloak.Authz.Net.Attributes;
using Keycloak.Authz.Net.Tests.Factories;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

public class PermissionsResolverTests
{
    [Fact]
    public void Should_Detect_Placeholders_In_Permissions()
    {
        var permission = "grade_{id}#update";
        var result = PermissionsResolver.DetectPlaceholders([permission]);
        result.Should().ContainKey(permission);
        result[permission].Should().Contain("id");
    }
    [Fact]
    public void Should_Detect_More_Than_One_Placeholder()
    {
        var permission = "{type}_{id}#update";
        var result = PermissionsResolver.DetectPlaceholders([permission]);
        result.Should().ContainKey(permission);
        result[permission].Should().Contain("id", "type");
    }
    [Fact]
    public void Permissions_Without_Placeholder_Should_Come_With_Empty_Detected_Placeholders_Array()
    {
        var permission = "grade#create";
        var result = PermissionsResolver.DetectPlaceholders([permission]);
        result.Should().ContainKey(permission);
        result[permission].Should().BeEmpty();
    }

    [Fact]
    public void Should_Detect_Placeholders_In_More_Than_One_Placeholders()
    {
        var permission1 = "{type}_{id}#update";
        var permission2 = "grade_{id}#update";
        var permission3 = "grade#create";
        var result = PermissionsResolver.DetectPlaceholders([permission1, permission2, permission3]);
        result.Should().ContainKey(permission1);
        result[permission1].Should().Contain("id", "type");
        result.Should().ContainKey(permission2);
        result[permission2].Should().Contain("id");
        result.Should().ContainKey(permission3);
        result[permission3].Should().BeEmpty();
    }

    [Fact]
    public void Should_Resolve_Permission_Placeholders_From_RequestHeader()
    {
        var permission = "grade_{tenant}#create";
        var httpContext = HttpContextFactory.Instance.BuildWithHeaders(new Dictionary<string, StringValues> {
            { "tenant", "tenant1" }
        });
        var result = new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Headers);
        result.Should().Contain("grade_tenant1#create");
    }

    [Fact]
    public void Should_Throw_Argument_Exception_When_A_Placeholder_Cannot_Be_Resolved_From_Headers()
    {
        var permission = "grade_{tenant}#create";
        var httpContext = HttpContextFactory.Instance.BuildWithHeaders([]);
        Action act = () => new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Headers);
        act.Should().Throw<ArgumentException>().WithMessage("The placeholder 'tenant' could not be resolved from the headers.");
    }

    [Fact]
    public void Should_Resolve_Permission_Placeholders_From_QueryString()
    {
        var permission = "grade_{type}#create";
        var httpContext = HttpContextFactory.Instance.BuildWithQueryString(new Dictionary<string, StringValues> {
            { "type", "semester" }
        });
        var result = new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Query);
        result.Should().Contain("grade_semester#create");
    }
    [Fact]
    public void Should_Throw_Argument_Exception_When_A_Placeholder_Cannot_Be_Resolved_From_Query()
    {
        var permission = "grade_{type}#create";
        var httpContext = HttpContextFactory.Instance.BuildWithQueryString([]);
        Action act = () => new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Query);
        act.Should().Throw<ArgumentException>().WithMessage("The placeholder 'type' could not be resolved from the query.");
    }
    [Fact]
    public void Should_Resolve_Permission_Placeholders_From_Route_Params()
    {
        var permission = "grade_{id}#create";
        var id = Guid.NewGuid().ToString();
        var httpContext = HttpContextFactory.Instance.BuildWithRouteParam("id", id);
        var result = new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Params);
        result.Should().Contain($"grade_{id}#create");
    }

    [Fact]
    public void Should_Throw_Argument_Exception_When_A_Placeholder_Cannot_Be_Resolved_From_Route_Params()
    {
        var permission = "grade_{id}#create";
        var httpContext = HttpContextFactory.Instance.BuildWithRouteParam("type", "semester");
        Action act = () => new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Params);
        act.Should().Throw<ArgumentException>().WithMessage("The placeholder 'id' could not be resolved from the route parameters.");
    }

    [Fact]
    public void Should_Resolve_Permission_Placeholders_From_Request_Body()
    {
        var permission = "grade_{id}#create";
        var id = Guid.NewGuid().ToString();
        var httpContext = HttpContextFactory.Instance.BuildWithBody(new Dictionary<string, string> {
            { "id", id }
        });
        var result = new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Body);
        result.Should().Contain($"grade_{id}#create");
    }

    [Fact]
    public void Should_Throw_Argument_Exception_When_A_Placeholder_Cannot_Be_Resolved_From_Body()
    {
        var permission = "grade_{id}#create";
        var httpContext = HttpContextFactory.Instance.BuildWithBody(new Dictionary<string, string> {
            { "type", "semester" }
        });
        Action act = () => new PermissionsResolver([permission], httpContext).Resolve(PlaceholderSource.Body);
        act.Should().Throw<ArgumentException>().WithMessage("The placeholder 'id' could not be resolved from the body.");
    }
}