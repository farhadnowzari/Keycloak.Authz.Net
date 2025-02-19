using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;

namespace Keycloak.Authz.Net.Tests.Factories;

public class HttpContextFactory
{
    private readonly Mock<HttpContext> HttpContextMock = new();

    public static HttpContextFactory Instance => new();

    public HttpContext Build()
    {
        return HttpContextMock.Object;
    }

    public HttpContext BuildWithHeaders(Dictionary<string, StringValues> headers)
    {
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Headers).Returns(new HeaderDictionary(headers));
        HttpContextMock.SetupGet(x => x.Request).Returns(requestMock.Object);
        return HttpContextMock.Object;
    }

    public HttpContext BuildWithQueryString(Dictionary<string, StringValues> query)
    {
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Query).Returns(new QueryCollection(query));
        HttpContextMock.SetupGet(x => x.Request).Returns(requestMock.Object);
        return HttpContextMock.Object;
    }

    public HttpContext BuildWithRouteParam(string key, string value)
    {
        var routingFeatureMock = new Mock<IRoutingFeature>();
        var routeValueMock = new Mock<IRouteValuesFeature>();
        var routeDictionary = new RouteValueDictionary {
            { key, value }
        };
        var routeData = new RouteData();
        routeData.Values[key] = value;
        routingFeatureMock.SetupGet(x => x.RouteData).Returns(routeData);
        routeValueMock.SetupGet(x => x.RouteValues).Returns(routeDictionary);
        var featureCollection = new FeatureCollection();
        featureCollection.Set(routingFeatureMock.Object);
        featureCollection.Set(routeValueMock.Object);
        HttpContextMock.SetupGet(x => x.Features).Returns(featureCollection);
        return HttpContextMock.Object;
    }

    public HttpContext BuildWithBody(Dictionary<string, string> body)
    {
        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body))));
        HttpContextMock.SetupGet(x => x.Request).Returns(requestMock.Object);
        return HttpContextMock.Object;
    }
}