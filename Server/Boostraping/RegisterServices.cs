using Server.Services;

namespace Server.Boostraping;

public static class RegisterServices
{
    public static IServiceCollection UseBoostraping(this IServiceCollection services, IConfiguration configuration)
    {   
        services.AddHostedService<WorkerServices>();
        services.AddControllers().AddOData(options =>
            options.Filter().Select().OrderBy().Count().SetMaxTop(100));
        services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddDbContext<FunewsManagementContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        });
        services.AddAutoMapper(typeof(MapProfile).Assembly);
        services.AddCors();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(configuration["JWT:Key"]
                        ?? throw new Exception("Key not found"))
                ),
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }

    public static WebApplication UseBoostraping(this WebApplication app)
    {
        app.UseCors(builder =>
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        return app;
    }
}