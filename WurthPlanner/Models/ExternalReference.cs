using System;
using System.Collections.Generic;
using System.Text;

namespace WurthPlanner.Models;

public class ExternalReference
{
    public required string Type { get; set; }
    public required string Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Url { get; set; }
}
