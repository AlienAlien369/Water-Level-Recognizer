namespace WLR.Domain.Common;
public abstract class AuditableEntity : BaseEntity
{
    public string? IpAddress { get; protected set; }
    public string? UserAgent { get; protected set; }
    public void SetRequestContext(string? ipAddress, string? userAgent)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}
