using System;

namespace Just.Grpc;

public class HostOptions
{
    public int Port { get; set; } = 8080;
    public bool Silent { get; set; } = false;
    public Action? Ready { get; set; } = null;
    public TransportLevelSecuritySettings? TransportLevelSecuritySettings { get; set; }
}