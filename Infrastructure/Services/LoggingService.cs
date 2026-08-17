using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xenon.Domain.Interfaces.Services;
using Xenon.Domain.Models;
using Xenon.Infrastructure.Data;

namespace Xenon.Infrastructure.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly StoreDbContext _context;
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(StoreDbContext context, ILogger<LoggingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(string level, string source, string message,
            string? exception = null, string? userId = null, string? userName = null,
            string? ipAddress = null, string? entityType = null, long? entityId = null,
            string? metadata = null)
        {
            switch (level)
            {
                case "Error":
                case "Critical":
                    _logger.LogError("[{Source}] {Message} {Exception}", source, message, exception ?? "");
                    break;
                case "Warning":
                    _logger.LogWarning("[{Source}] {Message}", source, message);
                    break;
                default:
                    _logger.LogInformation("[{Source}] {Message}", source, message);
                    break;
            }

            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Source = source,
                Message = message,
                Exception = exception,
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                EntityType = entityType,
                EntityId = entityId,
                Metadata = metadata,
                IsAlert = level is "Error" or "Critical"
            };

            _context.LogEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public Task LogInfoAsync(string source, string message,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? entityType = null, long? entityId = null, string? metadata = null)
            => LogAsync("Info", source, message, userId: userId, userName: userName,
                ipAddress: ipAddress, entityType: entityType, entityId: entityId, metadata: metadata);

        public Task LogWarningAsync(string source, string message,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? entityType = null, long? entityId = null, string? metadata = null)
            => LogAsync("Warning", source, message, userId: userId, userName: userName,
                ipAddress: ipAddress, entityType: entityType, entityId: entityId, metadata: metadata);

        public Task LogErrorAsync(string source, string message, Exception? ex = null,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? entityType = null, long? entityId = null, string? metadata = null)
            => LogAsync("Error", source, message, exception: ex?.ToString(),
                userId: userId, userName: userName, ipAddress: ipAddress,
                entityType: entityType, entityId: entityId, metadata: metadata);

        public async Task NotifyAsync(string title, string message, string type = "Info", string? link = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                Link = link
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync()
        {
            return await _context.Notifications.CountAsync(n => !n.IsRead);
        }

        public async Task<List<Notification>> GetNotificationsAsync(int count = 20)
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task MarkNotificationReadAsync(long id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllNotificationsReadAsync()
        {
            await _context.Notifications
                .Where(n => !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task<List<LogEntry>> GetLogsAsync(int page, int pageSize,
            string? level = null, string? source = null,
            DateTime? from = null, DateTime? to = null, string? search = null)
        {
            var query = _context.LogEntries.AsQueryable();

            if (!string.IsNullOrEmpty(level))
                query = query.Where(l => l.Level == level);
            if (!string.IsNullOrEmpty(source))
                query = query.Where(l => l.Source == source);
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(l => l.Message.Contains(search) || l.Source.Contains(search));

            return await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetLogsCountAsync(string? level = null, string? source = null,
            DateTime? from = null, DateTime? to = null, string? search = null)
        {
            var query = _context.LogEntries.AsQueryable();

            if (!string.IsNullOrEmpty(level))
                query = query.Where(l => l.Level == level);
            if (!string.IsNullOrEmpty(source))
                query = query.Where(l => l.Source == source);
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(l => l.Message.Contains(search) || l.Source.Contains(search));

            return await query.CountAsync();
        }
    }
}
