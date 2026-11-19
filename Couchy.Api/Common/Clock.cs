namespace Couchy.Api.Common;

public static class Clock
{
    /// <summary>
    /// Local time without a time zone, like Java's LocalDateTime.now().
    /// The columns are "timestamp without time zone", so the kind must not be UTC.
    /// </summary>
    public static DateTime Now() => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
}
