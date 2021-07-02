using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class WriteToFileHostedService : IHostedService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string fileName = "File 1.txt";
        private Timer timer;

        public WriteToFileHostedService(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Runs on application start
        public Task StartAsync(CancellationToken cancellationToken)
        {
            WriteToFile($"Process Started at {System.DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}");
            // timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5)); // Run the WriteFile method every 5 seconds
            return Task.CompletedTask;
        }

        // Runs on application close - Not 100% reliable, it may not run if the application closes suddenly (e.g application crash or power loss)
        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteToFile($"Process Stopped at {System.DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            WriteToFile($"Process Ongoing: {System.DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}");
        }

        // Custom method to write a line to file
        private void WriteToFile(string message)
        {
            var path = $@"{_env.ContentRootPath}\wwwroot\{fileName}";

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}