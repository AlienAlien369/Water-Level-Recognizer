namespace WLR.Infrastructure;

/// <summary>
/// Converts cloud-provider connection string URL formats (postgres://, redis://)
/// to the key-value format expected by Npgsql and StackExchange.Redis.
/// </summary>
public static class ConnectionStringHelper
{
    /// <summary>
    /// Converts a postgres:// or postgresql:// URL to Npgsql key-value format.
    /// No-ops if already in key-value format.
    /// </summary>
    public static string NormalizePostgres(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return connectionString ?? string.Empty;

        if (!connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.TrimStart('/');
        var port = uri.Port > 0 ? uri.Port : 5432;

        return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }

    /// <summary>
    /// Converts a redis:// or rediss:// URL to StackExchange.Redis connection string format.
    /// No-ops if already in key-value format.
    /// </summary>
    public static string NormalizeRedis(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return connectionString ?? string.Empty;

        if (!connectionString.StartsWith("redis://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        var uri = new Uri(connectionString);
        var isSsl = connectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase);
        var port = uri.Port > 0 ? uri.Port : (isSsl ? 6380 : 6379);

        var password = string.Empty;
        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var parts = uri.UserInfo.Split(':', 2);
            password = Uri.UnescapeDataString(parts.Length > 1 ? parts[1] : parts[0]);
        }

        var result = $"{uri.Host}:{port}";
        if (!string.IsNullOrEmpty(password))
            result += $",password={password}";
        if (isSsl)
            result += ",ssl=true,abortConnect=false";

        return result;
    }
}
