using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace AdinaCardGame
{
    public class LocalEntryPoint
    {
        public static void Main() =>
             WebHost.CreateDefaultBuilder()
                 .UseStartup<Startup>()
                 .Build()
                 .Run();
    }
}
