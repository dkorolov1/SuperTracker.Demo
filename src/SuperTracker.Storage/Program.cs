using MassTransit;
using System.Reflection;
using SuperTracker.Storage.Settings;
using SuperTracker.Storage.Services.PixelTrackStorageService;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<IPixelTrackStorageService, PixelTrackStorageService>();

// Configure log file settings.
builder.Services.Configure<LogFileSettings>(builder.Configuration.GetSection(LogFileSettings.Section));

// Configure MassTransit with RabbitMQ using settings from configuration.
builder.Services.AddMassTransit(conf => {
    conf.SetKebabCaseEndpointNameFormatter();
    conf.SetInMemorySagaRepositoryProvider();

    var assembly = Assembly.GetEntryAssembly();

    conf.AddConsumers(assembly);
    conf.AddSagaStateMachines(assembly);
    conf.AddSagas(assembly);
    conf.AddActivities(assembly);

    conf.UsingRabbitMq((ctx, cfg) =>
    {
        RabbitMqSettings rabbitMqSettings = new();
        builder.Configuration.Bind(RabbitMqSettings.Section, rabbitMqSettings);

        cfg.Host(rabbitMqSettings.Host, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.Run();