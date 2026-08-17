using Xenon.Domain.Models;

namespace Xenon.Domain.Interfaces.Services
{
    public interface ILoggingService
    {
        Task LogAsync(string level, string source, string message,
            string? exception = null, string? userId = null, string? userName = null,
            string? ipAddress = null, string? entityType = null, long? entityId = null,
            string? metadata = null);

        Task LogInfoAsync(string source, string message,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? entityType = null, long? entityId = null, string? metadata = null);

        Task LogWarningAsync(string source, string message,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? entityType = null, long? entityId = null, string? metadata = null);

        Task LogErrorAsync(string source, string message, Exception? ex = null,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? entityType = null, long? entityId = null, string? metadata = null);

        Task NotifyAsync(string title, string message, string type = "Info", string? link = null);
        Task<int> GetUnreadNotificationCountAsync();
        Task<List<Notification>> GetNotificationsAsync(int count = 20);
        Task MarkNotificationReadAsync(long id);
        Task MarkAllNotificationsReadAsync();

        Task<List<LogEntry>> GetLogsAsync(int page, int pageSize,
            string? level = null, string? source = null,
            DateTime? from = null, DateTime? to = null, string? search = null);
        Task<int> GetLogsCountAsync(string? level = null, string? source = null,
            DateTime? from = null, DateTime? to = null, string? search = null);
    }
}
