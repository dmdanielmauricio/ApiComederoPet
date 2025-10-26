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

                    // 🕒 Obtener hora actual de Colombia (UTC-5)
                    DateTime horaColombia;
                    try
                    {
                        var tz = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                        horaColombia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                    }
                    catch
                    {
                        // fallback (Render o Linux no siempre reconoce el nombre)
                        horaColombia = DateTime.UtcNow.AddHours(-5);
                    }

                    int hora = horaColombia.Hour;
                    int minuto = horaColombia.Minute;

                    // 🔍 Buscar horarios activos
                    var horarios = await db.FeedSchedules
                        .Where(h => h.IsActive)
                        .ToListAsync(stoppingToken);

                    foreach (var h in horarios)
                    {
                        // ✅ Coincidencia de hora exacta o dentro de ±1 minuto
                        bool coincide = (h.Hour == hora && Math.Abs(h.Minute - minuto) <= 1);

                        if (coincide)
                        {
                            var state = await db.FeedStates.FirstOrDefaultAsync(stoppingToken);
                            if (state != null && !state.ShouldFeed)
                            {
                                state.ShouldFeed = true;
                                state.LastFed = horaColombia;

                                // Registrar log automático
                                db.FeedLogs.Add(new Models.FeedLog
                                {
                                    Source = "auto",
                                    Timestamp = horaColombia
                                });

                                await db.SaveChangesAsync(stoppingToken);
                                _logger.LogInformation($"🐾 Alimentación automática disparada a las {horaColombia:HH:mm:ss}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error en SchedulerService");
                }

                // Espera 30 segundos antes de volver a revisar
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
