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
                                  builder.WithOrigins("http://127.0.0.1:7071").AllowAnyHeader().AllowAnyMethod();
                              });
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IBadgeRepository, BadgeRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<IFriendsRepository, FriendsRepository>();
        services.AddDbContext<ApplicationContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            options.UseMySql(connectionString, serverVersion);
        });
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IBadgeService, BadgeService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IFriendsService, FriendsService>();
    })
    .Build();
host.Run();

