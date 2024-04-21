using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Visuals;
using UnityServe.Views;

namespace UnityServe.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public void ShutdownRequested(object? sender, EventArgs args)
        {
            // MainWindow.GetTopLevel() ._server.Stop();
        }
    }
}
