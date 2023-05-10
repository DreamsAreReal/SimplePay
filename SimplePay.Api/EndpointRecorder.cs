namespace SimplePay.Api;

public record Endpoint(string Pattern, string[] HttpMethods, Delegate Handler);

public static class EndpointRecorder
{
    private static readonly List<Endpoint> _endpoints = new();
    private static IEndpointRouteBuilderWrapper? _routeBuilderWrapper;

    public static void Add(Endpoint endpoint)
    {
        _endpoints.Add(endpoint);
    }

    public static void RegisterAll(IEndpointRouteBuilder routeBuilder)
    {
        var routeBuilderWrapper = GetWrapper(routeBuilder);

        foreach (var endpoint in _endpoints)
        {
            routeBuilderWrapper.MapMethods
                (endpoint.Pattern, endpoint.HttpMethods, endpoint.Handler);
        }
    }

    private static IEndpointRouteBuilderWrapper GetWrapper(IEndpointRouteBuilder routeBuilder)
    {
        if (_routeBuilderWrapper is not null)
            return _routeBuilderWrapper;

        _routeBuilderWrapper = new EndpointRouteBuilderWrapper(routeBuilder);

        return _routeBuilderWrapper;
    }
}