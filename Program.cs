var AllowSpecificOrigins = "_allowSpecificOrigins";
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddCors(options =>
        {
            options.AddPolicy(name: AllowSpecificOrigins,
                              builder =>
                              {
                                  builder.WithOrigins(Environment.GetEnvironmentVariable("Host:CORS")).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                              });
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();
        services.AddScoped<IBadgeService, BadgeService>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<INotificationService, NotificationsService>();
    })
    .Build();
host.Run();
