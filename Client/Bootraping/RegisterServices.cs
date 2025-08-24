using Client.ControllerMvc;

namespace Client.Bootraping;

public static class RegisterServices
{
    public static IServiceCollection AddBootraping(this IServiceCollection builder)
    {
        builder.AddControllersWithViews();
        builder.AddHttpClient<HomeController>();
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
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=Index}/{id?}");
            
        return app;
    }
}