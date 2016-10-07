using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;
using TaskCat.App;

namespace TaskCat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Test out");
            string baseAddress = "localhost:8177";
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                Console.WriteLine();
                Console.WriteLine("Press ENTER to stop the server and close app...");
                Console.ReadLine();
            }
        }
    }
}