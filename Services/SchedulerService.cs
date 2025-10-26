using ApiComederoPet.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiComederoPet.Services
{
    public class SchedulerService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SchedulerService> _logger;

        public SchedulerService(IServiceProvider services, ILogger<SchedulerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🕐 Servicio de programación de comidas iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Obtener hora actual de Colombia
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                    var horaColombia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

                    var hora = horaColombia.Hour;
                    var minuto = horaColombia.Minute;

                    // Buscar horarios que coincidan (±1 minuto)
                    var horarios = await db.FeedSchedules
                        .Where(h => h.IsActive)
                        .ToListAsync();

                    foreach (var h in horarios)
                    {
                        if (Math.Abs(h.Hour - hora) == 0 && Math.Abs(h.Minute - minuto) <= 1)
                        {
                            var state = await db.FeedStates.FirstOrDefaultAsync();
                            if (state != null)
                            {
                                state.ShouldFeed = true;
                                state.LastFed = horaColombia;
                                await db.SaveChangesAsync();

                                _logger.LogInformation($"🐾 Alimentación automática disparada a las {horaColombia}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en SchedulerService");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
