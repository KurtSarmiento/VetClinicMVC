using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class SystemLog
{
    public long LogId { get; set; }

    public string LogLevel { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Exception { get; set; }

    public string? StackTrace { get; set; }

    public string? Source { get; set; }

    public DateTime? Timestamp { get; set; }
}
