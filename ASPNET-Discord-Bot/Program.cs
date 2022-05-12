using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Host
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Configure DI Services


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
