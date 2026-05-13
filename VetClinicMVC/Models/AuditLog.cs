using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class AuditLog
{
    public long LogId { get; set; }

    public string? UserId { get; set; }

    public string? Username { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public int? EntityId { get; set; }

    public string? Description { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? Ipaddress { get; set; }

    public string? UserAgent { get; set; }

    public string? PageUrl { get; set; }

    public string? Method { get; set; }

    public bool? Success { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime Timestamp { get; set; }
}
