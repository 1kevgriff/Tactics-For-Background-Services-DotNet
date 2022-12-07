var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<CleanUpTempFolderService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
