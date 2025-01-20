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
                                  builder.WithOrigins(Environment.GetEnvironmentVariable("AllowedOrigins")).AllowAnyHeader().AllowAnyMethod();
                              });
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IExerciseService, ExerciseService>();



        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<IFriendsRepository, FriendsRepository>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IBadgeService, BadgeService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IFriendsService, FriendsService>();
    })
    .Build();
host.Run();
