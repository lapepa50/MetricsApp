var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext ctx) => Results.Problem("Something went wrong."));
app.UseMiddleware<RequestTimingMiddleware>();

app.MapGet("/", () => "Hello World!");

app.MapGet("/metrics", () => Results.Json(RequestTimingMiddleware.GetLastRequests()));

app.Run();