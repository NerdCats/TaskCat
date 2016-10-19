using Its.Configuration;
using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using TaskCat.App.Settings;
using TaskCat.Lib.Constants;

namespace TaskCat
{
    public static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            string listeningAddress = string.IsNullOrWhiteSpace(Settings.Get<ClientSettings>().HostingAddress)
                ? AppConstants.DefaultHostingAddress : Settings.Get<ClientSettings>().HostingAddress;


            using (WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder)))
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Hosted TaskCat on {listeningAddress}");
                Console.ForegroundColor = oldColor;
                Console.ReadLine();
            }         
        }
    }
}
