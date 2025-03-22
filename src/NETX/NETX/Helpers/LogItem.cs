using Serilog.Events;

namespace NETX.Helpers
{
    public record LogItem(DateTimeOffset Timestamp, LogEventLevel Level, string Message);
}
