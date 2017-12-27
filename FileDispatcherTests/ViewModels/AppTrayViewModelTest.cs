using System.Linq;
using System.Windows;
using FileDispatcher;
using FileDispatcher.ViewModels;
using FileDispatcher.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class AppTrayViewModelTest
    {
        static App app;
        static AppTrayViewModel appTrayViewModel;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            app = new App();
            appTrayViewModel = new AppTrayViewModel(app);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            app.Shutdown();
            app = null;
        }

        [TestMethod]
        public void ShowLogCommand_LogWindowIsShown()
        {
            //Act
            appTrayViewModel.ShowLogCommand.Execute(null);
            //Assert
            AssertSingleWindowOfTypeIsPresent<LogWindow>(app.Windows);
        }

        [TestMethod]
        public void ShowLogCommand_CallTwice_SingleLogWindowIsShown()
        {
            //Act
            appTrayViewModel.ShowLogCommand.Execute(null);
            appTrayViewModel.ShowLogCommand.Execute(null);
            //Assert
            AssertSingleWindowOfTypeIsPresent<LogWindow>(app.Windows);
        }

        [TestMethod]
        public void ShowConfigurationCommand_ConfigurationWindowIsShown()
        {
            //Act
            appTrayViewModel.ShowConfigurationCommand.Execute(null);
            //Assert
            AssertSingleWindowOfTypeIsPresent<ConfigWindow>(app.Windows);
        }

        [TestMethod]
        public void ShowConfigurationCommand_CallTwice_SingleConfigurationWindowIsShown()
        {
            //Act
            appTrayViewModel.ShowConfigurationCommand.Execute(null);
            appTrayViewModel.ShowConfigurationCommand.Execute(null);
            //Assert
            AssertSingleWindowOfTypeIsPresent<ConfigWindow>(app.Windows);
        }

        private static void AssertSingleWindowOfTypeIsPresent<TWindow>(WindowCollection windowCollection) where TWindow : Window
        {
            var assertedWindows = windowCollection
                .OfType<TWindow>();
            Assert.IsTrue(assertedWindows.Count() == 1);
        }
    }
}
