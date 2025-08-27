using Client.ControllerMvc;

namespace Client.Bootraping;

public static class RegisterServices
{
    public static IServiceCollection AddBootraping(this IServiceCollection builder)
    {
        builder.AddControllersWithViews();
        builder.AddHttpClient<HomeController>();
        builder.AddHttpClient<LoginController>();
        builder.AddHttpClient<AdminAccountController>();
        builder.AddHttpClient<AdminCategoryController>();
        builder.AddHttpClient<AdminNewsArticleController>();
        builder.AddHttpClient<AdminTagController>();
        builder.AddHttpClient<UserAccountController>();
        builder.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.AddHttpContextAccessor();
        return builder;
    }
    
    public static WebApplication AddBootraping(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseSession();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
            
        return app;
    }
}