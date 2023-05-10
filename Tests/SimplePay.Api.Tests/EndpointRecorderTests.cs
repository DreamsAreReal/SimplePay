using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SimplePay.Api.Tests;

public class EndpointRecorderTests
{
    [Fact]
    public void Add_ShouldAddEndpointToList()
    {
        ClearEndpointsUsingReflection();

        // Arrange
        var endpoint = new Endpoint
            ("/test", new[] {HttpMethods.Get}, (Func<string>) (() => "test"));

        // Act
        EndpointRecorder.Add(endpoint);

        // Assert
        Assert.Contains(endpoint, GetAllEndpointsUsingReflection());
    }

    [Fact]
    public void RegisterAll_ShouldRegisterEndpointsInRouteBuilder()
    {
        ClearEndpointsUsingReflection();

        // Arrange
        var endpoints = new List<Endpoint>
        {
            new("/test", new[] {HttpMethods.Get}, (Func<string>) (() => "test")),
            new("/test1", new[] {HttpMethods.Get}, (Func<string>) (() => "test")),
            new("/test2", new[] {HttpMethods.Get}, (Func<string>) (() => "test")),
        };

        foreach (var endpoint in endpoints)
        {
            EndpointRecorder.Add(endpoint);
        }

        var routeBuilder = new Mock<IEndpointRouteBuilder>();
        var routeBuilderWrapper = new Mock<IEndpointRouteBuilderWrapper>();
        var endpointIndex = 0;

        routeBuilderWrapper.Setup
                           (
                               rb => rb.MapMethods
                               (
                                   It.IsAny<string>(),
                                   It.IsAny<IEnumerable<string>>(),
                                   It.IsAny<Delegate>()
                               )
                           )
                           .Callback<string, IEnumerable<string>, Delegate>
                           (
                               (pattern, methods, handler) =>
                               {
                                   var currentEndpoint = endpoints[endpointIndex++];
                                   Assert.Equal(currentEndpoint.Pattern, pattern);
                                   Assert.Equal(currentEndpoint.HttpMethods, methods.ToArray());

                                   Assert.Equal
                                   (
                                       ((Func<string>) currentEndpoint.Handler)(),
                                       ((Func<string>) handler)()
                                   );
                               }
                           );

        // Act
        SetWrapperUsingReflection(routeBuilderWrapper.Object);
        EndpointRecorder.RegisterAll(routeBuilder.Object);

        // Assert
        routeBuilderWrapper.Verify
        (
            rb => rb.MapMethods
                (It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<Delegate>()),
            Times.Exactly(endpoints.Count)
        );
    }

    private void SetWrapperUsingReflection(IEndpointRouteBuilderWrapper routeBuilderWrapper)
    {
        var fieldName = "_routeBuilderWrapper";

        var fieldInfo = typeof(EndpointRecorder).GetField
            (fieldName, BindingFlags.Static | BindingFlags.NonPublic);

        if (fieldInfo is not null)
        {
            fieldInfo.SetValue(null, routeBuilderWrapper);

            return;
        }

        throw new InvalidOperationException($"Field {fieldName} not found");
    }

    #region EndpointsMethods

    private void ClearEndpointsUsingReflection()
    {
        var fieldInfo = GetEndpointsField();
        var endpoints = fieldInfo.GetValue(null) as List<Endpoint>;
        endpoints?.Clear();
    }

    private FieldInfo GetEndpointsField()
    {
        var fieldName = "_endpoints";

        var fieldInfo = typeof(EndpointRecorder).GetField
            (fieldName, BindingFlags.Static | BindingFlags.NonPublic);

        if (fieldInfo is null)
        {
            throw new InvalidOperationException($"Field {fieldName} not found");
        }

        return fieldInfo;
    }

    private List<Endpoint> GetAllEndpointsUsingReflection()
    {
        var fieldInfo = GetEndpointsField();

        return fieldInfo.GetValue(null) as List<Endpoint> ?? throw new InvalidOperationException();
    }

    #endregion
}