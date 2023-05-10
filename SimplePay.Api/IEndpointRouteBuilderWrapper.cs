namespace SimplePay.Api;

public interface IEndpointRouteBuilderWrapper
{
    IEndpointConventionBuilder MapMethods(
        string pattern,
        IEnumerable<string> httpMethods,
        Delegate requestDelegate
    );
}