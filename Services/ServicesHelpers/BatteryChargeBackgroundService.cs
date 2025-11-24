using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Services.BatteryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers
{
    public class BatteryChargeBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BatteryChargeBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var batteryService = scope.ServiceProvider.GetRequiredService<IBatteryService>();
                    var result = await batteryService.AutoChargeAsync();
                    Console.WriteLine($"[AutoCharge] {result.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
