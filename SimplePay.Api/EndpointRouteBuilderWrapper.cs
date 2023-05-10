namespace SimplePay.Api;

public class EndpointRouteBuilderWrapper : IEndpointRouteBuilderWrapper
{
    private readonly IEndpointRouteBuilder _routeBuilder;

    public EndpointRouteBuilderWrapper(IEndpointRouteBuilder routeBuilder)
    {
        _routeBuilder = routeBuilder;
    }

    public IEndpointConventionBuilder MapMethods(
        string pattern,
        IEnumerable<string> httpMethods,
        Delegate requestDelegate
    )
    {
        return _routeBuilder.MapMethods(pattern, httpMethods, requestDelegate);
    }
}