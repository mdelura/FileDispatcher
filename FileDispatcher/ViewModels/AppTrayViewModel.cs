using System.Windows;
using System.Windows.Input;
using FileDispatcher.Views;
using Gat.Controls;
using Prism.Commands;

namespace FileDispatcher.ViewModels
{
    class AppTrayViewModel
    {
        App _app;

        public AppTrayViewModel(App app)
        {
            _app = app;
            ShowLogCommand = new DelegateCommand(() => ShowAsMainWindow<LogWindow>());
            ShowConfigurationCommand = new DelegateCommand(() => ShowAsMainWindow<ConfigWindow>());
            ShowAboutCommand = new DelegateCommand(() => new About() { AdditionalNotes = null }.Show());
            ExitCommand = new DelegateCommand(() => _app.Shutdown());
        }

        public AppTrayViewModel() : this((App)Application.Current)
        {
        }

        /// <summary>
        /// Shows log.
        /// </summary>
        public ICommand ShowLogCommand { get; private set; }
        /// <summary>
        /// Shows configuration.
        /// </summary>
        public ICommand ShowConfigurationCommand { get; private set; }
        /// <summary>
        /// Shows About window.
        /// </summary>
        public ICommand ShowAboutCommand { get; private set; }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        private void ShowAsMainWindow<TWindow>() where TWindow : Window, new()
        {
            foreach (Window window in _app.Windows)
            {
                if (window is TWindow)
                {
                    _app.MainWindow = window;
                    if (window.WindowState == WindowState.Minimized)
                        window.WindowState = WindowState.Normal;
                    window.Activate();

                    return;
                }
            }
            new TWindow().Show();
        }
    }
}