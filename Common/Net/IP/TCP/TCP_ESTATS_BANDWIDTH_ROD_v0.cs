﻿namespace Wokhan.WindowsFirewallNotifier.Common.Net.IP.TCP;

public struct TCP_ESTATS_BANDWIDTH_ROD_v0
{
    public ulong OutboundBandwidth;
    public ulong InboundBandwidth;
    public ulong OutboundInstability;
    public ulong InboundInstability;
    public bool OutboundBandwidthPeaked;
    public bool InboundBandwidthPeaked;
}
