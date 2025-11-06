var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("<div> Hello FPoly from the middleware 1 </div>");
    await next.Invoke();
    await context.Response.WriteAsync("<div> Returning from the middleware 1 </div>");
});

app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("<div> Hello FPoly from the middleware 2 </div>");
    await next.Invoke();
    await context.Response.WriteAsync("<div> Returning from the middleware 2 </div>");
});

app.Run(async (context) =>
{
    await context.Response.WriteAsync("<div> Hello FPoly from the middleware 3 </div>");
});

app.MapGet("/", () => "Phạm Trần Anh Quân");

app.Run();
