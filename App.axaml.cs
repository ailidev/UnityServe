using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using UnityServe.ViewModels;
using UnityServe.Views;

namespace UnityServe
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            MainWindowViewModel mainWindowViewModel = new();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                desktop.ShutdownRequested += mainWindowViewModel.ShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
