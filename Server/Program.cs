using Server.Boostraping;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.UseBoostraping(builder.Configuration);
            var app = builder.Build();
            app.UseBoostraping();
            app.Run();
        }
    }
}                                     
