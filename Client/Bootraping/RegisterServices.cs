using Client.ControllerMvc;

namespace Client.Bootraping;

public static class RegisterServices
{
    public static IServiceCollection AddBootraping(this IServiceCollection builder)
    {
        builder.AddControllersWithViews();
        builder.AddHttpClient<LoginController>();
        builder.AddHttpClient<HomeController>();
        builder.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(3);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.AddDistributedMemoryCache();
        return builder;
    }
    
    public static WebApplication AddBootraping(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");
            
        return app;
    }
}