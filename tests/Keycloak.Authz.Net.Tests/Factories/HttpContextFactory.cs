using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;

namespace Keycloak.Authz.Net.Tests.Factories;

public class HttpContextFactory
{
    private static readonly Mock<HttpContext> HttpContextMock = new();

    public static HttpContext Build()
    {
        return HttpContextMock.Object;
    }

    public static HttpContext BuildWithHeaders(Dictionary<string, StringValues> headers)
    {
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Headers).Returns(new HeaderDictionary(headers));
        HttpContextMock.SetupGet(x => x.Request).Returns(requestMock.Object);
        return HttpContextMock.Object;
    }

    public static HttpContext BuildWithQueryString(Dictionary<string, StringValues> query)
    {
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Query).Returns(new QueryCollection(query));
        HttpContextMock.SetupGet(x => x.Request).Returns(requestMock.Object);
        return HttpContextMock.Object;
    }

    public static HttpContext BuildWithRouteParam(string key, string value)
    {
        var routingFeatureMock = new Mock<IRoutingFeature>();
        var routeData = new RouteData();
        routeData.Values[key] = value;
        routingFeatureMock.SetupGet(x => x.RouteData).Returns(routeData);
        var featureCollection = new FeatureCollection();
        featureCollection.Set(routingFeatureMock.Object);
        HttpContextMock.SetupGet(x => x.Features).Returns(featureCollection);
        return HttpContextMock.Object;
    }

    public static HttpContext BuildWithBody(Dictionary<string, string> body)
    {
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body))));
        HttpContextMock.SetupGet(x => x.Request).Returns(requestMock.Object);
        return HttpContextMock.Object;
    }
}