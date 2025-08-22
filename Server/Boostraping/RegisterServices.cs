namespace Server.Boostraping;

public static class RegisterServices
{
    public static IServiceCollection UseBoostraping(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddOData(options =>
            options.Filter().Select().OrderBy().Count().SetMaxTop(100));
        services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
        services.AddDbContext<FunewsManagementContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        });
        services.AddAutoMapper(typeof(MapProfile).Assembly);
        services.AddCors();
        return services;
    }

    public static WebApplication UseBoostraping(this WebApplication app)
    {
        app.MapControllers();
        
        return app;
    }
}