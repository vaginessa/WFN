﻿
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Wokhan.UI.Extensions;
using Wokhan.WindowsFirewallNotifier.Common.Config;
using Wokhan.WindowsFirewallNotifier.Common.Helpers;
using Wokhan.WindowsFirewallNotifier.Common.Logging;
using Wokhan.WindowsFirewallNotifier.Common.Processes;
using Wokhan.WindowsFirewallNotifier.Common.Security;
using Wokhan.WindowsFirewallNotifier.Common.UI.ViewModels;

namespace Wokhan.WindowsFirewallNotifier.Console.UI.Pages;

[ObservableObject]
public sealed partial class EventsLog : Page, IDisposable
{
    public EventLogAsyncReader<LogEntryViewModel> EventsReader { get; set; }

    public ICollectionView dataView;

    private readonly EventsLogFilters eventsLogFilters;

    public int TCPOnlyOrAll
    {
        get => IsTCPOnlyEnabled ? 1 : 0;
        set => IsTCPOnlyEnabled = (value == 1);
    }

    public bool IsTCPOnlyEnabled
    {
        get => Settings.Default.FilterTcpOnlyEvents;
        set
        {
            if (IsTCPOnlyEnabled != value)
            {
                Settings.Default.FilterTcpOnlyEvents = value;
                Settings.Default.Save();
                eventsLogFilters.ResetTcpFilter();
            }
        }
    }

    public string FilterText
    {
        get => eventsLogFilters.FilterText;
        set
        {
            eventsLogFilters.FilterText = value;
            eventsLogFilters.ResetTextfilter();
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LocateCommand))]
    private LogEntryViewModel selectedItem;

    public EventsLog()
    {
        VirtualizedQueryableExtensions.Init(Dispatcher);

        if (UAC.CheckProcessElevated())
        {
            Loaded += (s, e) => StartHandlingSecurityLogEvents();
            Unloaded += (s, e) => StopHandlingSecurityLogEvents();
        }

        eventsLogFilters = new EventsLogFilters(this);

        InitializeComponent();

        if (!Settings.Default.EnableDnsResolver)
        {
            RemoteHostCol.Visibility = Visibility.Hidden;
        }
    }

    private void StartHandlingSecurityLogEvents(bool reset = false)
    {
        try
        {
            if (reset || EventsReader is null)
            {
                EventsReader?.Dispose();
                EventsReader = new EventLogAsyncReader<LogEntryViewModel>(EventLogAsyncReader.EVENTLOG_SECURITY, LogEntryViewModel.CreateFromEventLogEntry)
                {
                    FilterPredicate = EventLogAsyncReader.IsFirewallEvent
                };
                OnPropertyChanged(nameof(EventsReader));

                dataView = CollectionViewSource.GetDefaultView(EventsReader.Entries);
                gridLog.ItemsSource = dataView;
            }

            //eventsLogFilters.ResetTcpFilter();
        }
        catch (Exception exc)
        {
            LogHelper.Error("Unable to connect to the event log", exc);
            throw;
        }
    }

    private void StopHandlingSecurityLogEvents()
    {
        EventsReader?.Dispose();
        EventsReader = null;
    }

    public void Dispose()
    {
        EventsReader?.Dispose();
    }

    [RelayCommand(CanExecute = nameof(LocateCanExecute))]
    private void Locate()
    {
        ProcessHelper.StartShellExecutable("explorer.exe", "/select," + SelectedItem.Path, true);
    }

    public bool LocateCanExecute => SelectedItem is not null;

    [RelayCommand]
    private void OpenEventsLogViewer()
    {
        ProcessHelper.StartShellExecutable("eventvwr.msc", null, true);
    }

    [RelayCommand]
    private void Refresh()
    {
        StartHandlingSecurityLogEvents(true);
    }


}
