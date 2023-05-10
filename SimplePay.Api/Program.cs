var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapMethods("test", new[] {HttpMethods.Get}, () => "s");
app.Run();