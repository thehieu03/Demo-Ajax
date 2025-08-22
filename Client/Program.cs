using Client.Bootraping;
using Client.ControllerMvc;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddBootraping();

            var app = builder.Build();
            app.AddBootraping();
            app.Run();
        }
    }
}
