using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Azure clients (Managed Identity via DefaultAzureCredential)
builder.Services.AddSingleton<JobWorker>();

var host = builder.Build();

var worker = host.Services.GetRequiredService<JobWorker>();
await worker.RunAsync();