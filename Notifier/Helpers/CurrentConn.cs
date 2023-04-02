﻿using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.Generic;
using System.ComponentModel;
using Wokhan.WindowsFirewallNotifier.Common.Config;
using Wokhan.WindowsFirewallNotifier.Common.UI.ViewModels;

namespace Wokhan.WindowsFirewallNotifier.Notifier.Helpers;

public partial class CurrentConn : LogEntryViewModel, INotifyPropertyChanged
{
    //private string _currentAppPkgId;
    //TODO: rename as it's not something "current"
    public string CurrentAppPkgId { get; set; }// => this.GetOrSetAsyncValue(() => ProcessHelper.GetAppPkgId(Pid), NotifyPropertyChanged, nameof(_currentAppPkgId));

    //private string _currentLocalUserOwner;
    //TODO: rename as it's not something "current"
    public string CurrentLocalUserOwner { get; set; }// => this.GetOrSetAsyncValue(() => ProcessHelper.GetLocalUserOwner(Pid), NotifyPropertyChanged, nameof(_currentLocalUserOwner));

    public SortedSet<int> LocalPortArray { get; } = new SortedSet<int>();
    
    //public string TargetInfoUrl => $"http://whois.domaintools.com/{Target}";  // uses captcha validation :(
    //public string TargetInfoUrl => $"https://bgpview.io/ip/{Target}";
    public string TargetInfoUrl => string.Format(Settings.Default.TargetInfoUrl, TargetIP);  // eg: $"https://bgpview.io/ip/{Target}"
    public string TargetPortUrl => string.Format(Settings.Default.TargetPortUrl, TargetPort); // eg: $"https://www.speedguide.net/port.php?port={TargetPort}"

    //TODO: remove since it's now useless
    public string[] PossibleServices { get; set; }
    //TODO: remove since it's now useless
    public string[] PossibleServicesDesc { get; set; }

    [ObservableProperty]
    private int _tentativesCounter = 1;
}
