using FluentDev.MethodUsedBy.Core;
using FluentDev.MethodUsedBy.Core.Analyze;
using FluentDev.MethodUsedBy.Core.Cache;
using FluentDev.MethodUsedBy.Core.Error;
using FluentDev.MethodUsedBy.Core.Logging;
using FluentDev.MethodUsedBy.Core.Report;
using FluentDev.MethodUsedBy.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// https://stackoverflow.com/questions/46569270/asp-net-core-configuration-reloadonchange-with-ioptionssnapshot-still-not-respon
var hostBuilder = Host.CreateDefaultBuilder(args);
hostBuilder.ConfigureAppConfiguration(config =>
{
    config.AddCommandLine(args, new Dictionary<string, string>
    {
        { "-a", "AssemblyDir" },
        { "-k", "Keyword" },
    });
});

hostBuilder.ConfigureLogging(logging => 
{
    logging.AddFileLogger();
});

hostBuilder.ConfigureServices((hostingContext, services) =>
{
    // https://blog.shibayan.jp/entry/20160529/1464456800
    // https://gist.github.com/yfakariya/ff6cd9653509181c8191f875e45be80f
    services.Configure<AppSettings>(hostingContext.Configuration);

    // App
    services.AddHostedService<AnalyzerHostService>();
    services.AddTransient<AnalyzeService>();
    services.AddTransient<SettingEnsuringService>();

    // Error
    services.AddTransient<ErrorHandler>();

    // Cache
    services.AddTransient<CacheLoaderFactory>();
    services.AddTransient<CacheAssemblyLoader>();
    services.AddTransient<CacheFileStore>();
    services.AddTransient<FileStoreFactory>();
    services.AddTransient<CacheFilter>();

    // Analyzer
    services.AddTransient<MethodReferenceAnalyzer>();

    // Report
    services.AddTransient<ReportTextStore>();

});

await hostBuilder.RunConsoleAsync();