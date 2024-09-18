using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskPro.Data;
using TaskPro.Data.Enums;

namespace TaskPro.Services
{
    public class TaskNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TaskNotificationService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendNotifications();
                // Espera 5 minutos antes de ejecutar de nuevo
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task SendNotifications()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                var tareas = await context.Tareas
                    .Include(t => t.Proyecto)
                    .ThenInclude(p => p.Usuario)
                    .Where(t => t.FechaVencimiento.Date == DateTime.Now.AddDays(1).Date && t.Estado == Estado.Pendiente)
                    .ToListAsync();

                foreach (var tarea in tareas)
                {
                    // Enviar notificación al usuario propietario de la tarea
                    await _hubContext.Clients.User(tarea.Proyecto!.Usuario!.UserName!.ToString())
                        .SendAsync("ReceiveNotification", $"La tarea '{tarea.Nombre}' vence mañana.");
                }
            }
        }
    }
}